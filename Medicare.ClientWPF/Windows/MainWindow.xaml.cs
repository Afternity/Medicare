using Medicare.ClientWPF.Consts;
using Medicare.ClientWPF.DTOs;
using Medicare.ClientWPF.Profile;
using Medicare.Domain.Data.DbContexts;
using Medicare.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Medicare.ClientWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
        : Window
    {
        private readonly MedicareDbContext _context;

        private AppointmentDto _appointmentDto = new AppointmentDto();
        private ObservableCollection<Appointment> _appointmentList = [];

        public MainWindow()
        {
            InitializeComponent();
            _context = new MedicareDbContext();
        }

        private async void Window_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            ListBoxUI.ItemsSource = await GetAllAsync();
        }

        private async void DeepSearch_Click(
            object sender,
            RoutedEventArgs e)
        {
            _appointmentDto.AppointmentDate = AppointmentDateDatePicker.SelectedDate ?? DateTime.MinValue;
            _appointmentDto.PatientFullName = PatientFullNameTextBox.Text;
            _appointmentDto.PatientPhone = PatientPhoneTextBox.Text;
            _appointmentDto.AppointmentNote = AppointmentNoteTextBox.Text;

            ListBoxUI.ItemsSource = await DeepSearch(_appointmentDto);
        }

        private void MedicalCardWindow_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (ListBoxUI.SelectedItem is not Appointment selected)
            {
                MessageBox.Show("Выберите пациента");
                return;
            }

            var window = new MedicalCardWindow();
            window.Show();
        }

        private void RecipeWindow_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (ListBoxUI.SelectedItem is not Appointment selected)
            {
                MessageBox.Show("Выберите пациента");
                return;
            }

            var window = new RecipeWindow();
            window.Show();
        }

        private void ProfileWindow_Click(
          object sender,
          RoutedEventArgs e)
        {
            var window = new ProfileWindow();
            window.Show();
        }

        private async void ListBox_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (ListBoxUI.SelectedItem is Appointment selected)
            {
                AppointmentIdTextBlock.Text = selected.Id.ToString();
                AppointmentDateTextBlock.Text = selected.AppointmentDate.ToString();
                PatientFullNameTextBlock.Text = selected.Patient.FullName;
                PatientPhoneTextBlock.Text = selected.Patient.Phone;
                AppointmentStatusTextBlock.Text = selected.Status;
                AppointmentNotesTextBlock.Text = selected.Notes;

                await GetPatientProfile(selected.PatientId);
            }
        }

        private async Task GetPatientProfile(
            Guid id)
        {
            try
            {
                using var tokenSource = new CancellationTokenSource(
                    ConstansOperations.TokenTimeLimit);

                var entity = await _context.Patients
                    .FirstOrDefaultAsync(patient =>
                        patient.Id == id,
                        tokenSource.Token);

                if (entity == null)
                {
                    MessageBox.Show("Профиль пациента не загрузился");
                    return;
                }

                PatientProfile.Profile = entity;
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Лимит времени");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private async Task<IList<Appointment>> GetAllAsync()
        {
            try
            {
                using var tokenSource = new CancellationTokenSource(
                    ConstansOperations.TokenTimeLimit);

                var entities = await _context.Appointments
                    .AsNoTracking()
                    .Include(appointment => appointment.Patient)
                    .Where(appointment =>
                        appointment.Doctor.Id == DoctorProfile.Profile.Id)
                    .ToListAsync(tokenSource.Token);

                return entities;
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Лимит времени");
                return new List<Appointment>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                return new List<Appointment>();
            }
        }

        private async Task<IList<Appointment>> DeepSearch(
            AppointmentDto modelDto)
        {
            try
            {
                using var tokenSource = new CancellationTokenSource(
                    ConstansOperations.TokenTimeLimit);

                var entities = await _context.Appointments
                    .AsNoTracking()
                    .Include(appointment => appointment.Patient)
                    .Include(appointment => appointment.Doctor)
                    .Where(appointment =>
                        appointment.Doctor.Id == DoctorProfile.Profile.Id
                        && (string.IsNullOrWhiteSpace(modelDto.PatientFullName)
                            || appointment.Patient.FullName.Contains(modelDto.PatientFullName))
                        && (string.IsNullOrWhiteSpace(modelDto.AppointmentNote)
                            || appointment.Notes.Contains(modelDto.AppointmentNote))
                        && (string.IsNullOrWhiteSpace(modelDto.PatientPhone)
                            || appointment.Patient.Phone.Contains(modelDto.PatientPhone))
                        && (modelDto.AppointmentDate == DateTime.MinValue
                            || appointment.AppointmentDate.Date == modelDto.AppointmentDate.Date))
                    .ToListAsync(tokenSource.Token);

                return entities;
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Лимит времени");
                return new List<Appointment>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                return new List<Appointment>();
            }
        }
    }
}
