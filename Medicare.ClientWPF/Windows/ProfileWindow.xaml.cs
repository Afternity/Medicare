using Medicare.ClientWPF.Consts;
using Medicare.ClientWPF.Profile;
using Medicare.Domain.Data.DbContexts;
using Medicare.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Medicare.ClientWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        private readonly MedicareDbContext _context;

        private Doctor _doctor = new Doctor();

        public ProfileWindow()
        {
            InitializeComponent();
            _context = new MedicareDbContext();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FullNameTextBlock.Text = DoctorProfile.Profile.FullName;
            EmailTextBlock.Text = DoctorProfile.Profile.Email;
            PasswordTextBlock.Text = DoctorProfile.Profile.Password;
            PhoneTextBlock.Text = DoctorProfile.Profile.Phone;
            DoctorTypeTextBlock.Text = DoctorProfile.Profile.DoctorType.Name;

            int count = await PatientCountAsync();

            PatientCountTextBlock.Text = $"Количество моих пациентов: {count}";
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            DoctorProfile.Profile.Email = EmailTextBlock.Text;    
            DoctorProfile.Profile.Password = PasswordTextBlock.Text;    
            DoctorProfile.Profile.Phone = PhoneTextBlock.Text;

            await UpdateProfileAsync(DoctorProfile.Profile);
        }

        private async Task<int> PatientCountAsync()
        {
            try
            {
                using var tokenSourse = new CancellationTokenSource(
                    ConstansOperations.TokenTimeLimit);

                var entities = await _context.Appointments
                    .AsNoTracking()
                    .Include(appointment => appointment.Doctor)
                    .Where(appointment =>
                        appointment.Doctor.Id == DoctorProfile.Profile.Id)
                    .ToListAsync(tokenSourse.Token);

                return entities.Count;
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Лимин по времени");
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                return 0;
            }
        }

        private async Task UpdateProfileAsync(Doctor model)
        {
            try
            {
                using var tokenSourse = new CancellationTokenSource(
                    ConstansOperations.TokenTimeLimit);

                var entity = await _context.Doctors
                    .FirstOrDefaultAsync(doctor =>
                        doctor.Id == DoctorProfile.Profile.Id,
                        tokenSourse.Token);

                if (entity == null)
                {
                    MessageBox.Show("Профиль для обновления не найден");
                    return;
                }

                entity.Email = model.Email;
                entity.Password = model.Password;
                entity.Phone = model.Phone;

                _context.Update(entity);
                await _context.SaveChangesAsync(tokenSourse.Token);

                MessageBox.Show("Профиль обновилься успешно");
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Лимин по времени");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            var window = new AuthWindow();
            window.Show();

            foreach (var otherWindow in Application.Current.Windows.OfType<Window>().ToList())
            {
                if (otherWindow is not AuthWindow)
                    otherWindow.Close();
            }
        }
    }
}
