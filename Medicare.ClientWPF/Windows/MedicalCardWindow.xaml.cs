using Medicare.ClientWPF.Consts;
using Medicare.ClientWPF.DTOs;
using Medicare.ClientWPF.Profile;
using Medicare.Domain.Data.DbContexts;
using Medicare.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Medicare.ClientWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для MedicalCardWindow.xaml
    /// </summary>
    public partial class MedicalCardWindow : Window
    {
        private readonly MedicareDbContext _context;

        public MedicalCardWindow()
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

        private void Back_Click(
            object sender,
            RoutedEventArgs e)
        {
            this.Close();
        }

        private void ListBox_SelectionChanged(
            object sender, 
            SelectionChangedEventArgs e)
        {
            if (ListBoxUI.SelectedItem is MedicalCard selected)
            {
                MedicalCardIdTextBlock.Text = selected.Id.ToString();
                PatientFullNameTextBlock.Text = selected.Patient.FullName;
                MedicalCardMedicalHistoryTextBlock.Text = selected.MedicalHistory;
                MedicalCardAllergiesTextBlock.Text = selected.Allergies;
                MedicalCardCurrentMedicationsTextBlock.Text = selected.CurrentMedications;
            }
        }

        private async void MedicalCardUpdate_Click(
            object sender,
            RoutedEventArgs e)
        {
            var modelDto = new MedicalCardDto()
            {
                Id = Guid.Parse(MedicalCardIdTextBlock.Text),
                MedicalHistory = MedicalCardMedicalHistoryTextBlock.Text,
                Allergies = MedicalCardAllergiesTextBlock.Text,
                CurrentMedications = MedicalCardCurrentMedicationsTextBlock.Text
            };

            await UpdateAsync(modelDto);
        }


        private async Task UpdateAsync(MedicalCardDto modelDto)
        {
            try
            {
                using var tokenSource = new CancellationTokenSource(
                    ConstansOperations.TokenTimeLimit);

                var entity = await _context.MedicalCards
                    .FirstOrDefaultAsync(medicalCard =>
                        medicalCard.Id == modelDto.Id,
                        tokenSource.Token);

                if (entity == null)
                {
                    MessageBox.Show("Запись не найдена");
                    return;
                }

                entity.MedicalHistory = modelDto.MedicalHistory;
                entity.CurrentMedications = modelDto.CurrentMedications;
                entity.Allergies = modelDto.Allergies;

                _context.Update(entity);
                await _context.SaveChangesAsync(tokenSource.Token);

                MessageBox.Show("Данные успешно обновились");

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

        private async Task<IList<MedicalCard>> GetAllAsync()
        {
            try
            {
                using var tokenSource = new CancellationTokenSource(
                    ConstansOperations.TokenTimeLimit);

                var entities = await _context.MedicalCards
                    .AsNoTracking()
                    .Include(medicalCard => medicalCard.Patient)
                    .Where(medicalCard =>
                        medicalCard.Patient.Id == PatientProfile.Profile.Id)
                    .ToListAsync(tokenSource.Token);

                return entities;
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Лимит времени");
                return new List<MedicalCard>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                return new List<MedicalCard>();
            }
        }
    }
}
