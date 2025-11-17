using Medicare.ClientWPF.Consts;
using Medicare.ClientWPF.DTOs;
using Medicare.ClientWPF.Profile;
using Medicare.Domain.Data.DbContexts;
using Medicare.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;


namespace Medicare.ClientWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для RecipeWindow.xaml
    /// </summary>
    public partial class RecipeWindow : Window
    {
        private readonly MedicareDbContext _context;

        public RecipeWindow()
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
            if (ListBoxUI.SelectedItem is Recipe selected)
            {
                RecipeIdTextBlock.Text = selected.Id.ToString();
                PatientFullNameTextBlock.Text = selected.Patient.FullName;
                IssueDateTextBlock.Text = selected.IssueDate.ToString("dd.MM.yyyy");
                RecipeMedicationTextBox.Text = selected.Medication;
                RecipeDosageTextBox.Text = selected.Dosage;
                RecipeInstructionsTextBox.Text = selected.Instructions;
                RecipeDurationDaysTextBox.Text = selected.DurationDays.Days.ToString();
            }
        }

        private void RecipeAdd_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (ListBoxUI.SelectedItem is not Recipe selected)
            {
                MessageBox.Show("Выберите запись");
                return;
            }

            var window = new RecipeAddWindow();
            window.Show();
        }

        private async void RecipeUpdate_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (ListBoxUI.SelectedItem is not Recipe selected)
            {
                MessageBox.Show("Выберите запись");
                return;
            }

            if (!Guid.TryParse(RecipeIdTextBlock.Text, out var recipeId))
            {
                MessageBox.Show("Неверный ID рецепта");
                return;
            }

            if (!int.TryParse(RecipeDurationDaysTextBox.Text, out var durationDays))
            {
                MessageBox.Show("Неверный формат длительности");
                return;
            }

            var modelDto = new RecipeDto()
            {
                Id = recipeId,
                Medication = RecipeMedicationTextBox.Text,
                Dosage = RecipeDosageTextBox.Text,
                Instructions = RecipeInstructionsTextBox.Text,
                DurationDays = TimeSpan.FromDays(durationDays)
            };

            await UpdateAsync(modelDto);
        }

        private async void RecipeDelete_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (ListBoxUI.SelectedItem is not Recipe selected)
            {
                MessageBox.Show("Выберите запись");
                return;
            }

            await DeleteAsync(selected);
        }

        private async void GetAll_Click(
            object sender,
            RoutedEventArgs e)
        {
            ListBoxUI.ItemsSource = await GetAllAsync();
        }

        private async Task DeleteAsync(Recipe model)
        {
            try
            {
                using var tokenSource = new CancellationTokenSource(
                    ConstansOperations.TokenTimeLimit);

                var entity = await _context.Recipes
                    .FirstOrDefaultAsync(recipe =>
                        recipe.Id == model.Id,
                        tokenSource.Token);

                if (entity == null)
                {
                    MessageBox.Show("Рецепт не найден");
                    return;
                }

                _context.Remove(entity);
                await _context.SaveChangesAsync(tokenSource.Token);

                MessageBox.Show("Рецепт успешно обновлен");

                // Обновляем список
                ListBoxUI.ItemsSource = await GetAllAsync();
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

        private async Task UpdateAsync(
            RecipeDto modelDto)
        {
            try
            {
                using var tokenSource = new CancellationTokenSource(
                    ConstansOperations.TokenTimeLimit);

                var entity = await _context.Recipes
                    .FirstOrDefaultAsync(recipe =>
                        recipe.Id == modelDto.Id,
                        tokenSource.Token);

                if (entity == null)
                {
                    MessageBox.Show("Рецепт не найден");
                    return;
                }

                entity.Medication = modelDto.Medication;
                entity.Dosage = modelDto.Dosage;
                entity.Instructions = modelDto.Instructions;
                entity.DurationDays = modelDto.DurationDays;

                _context.Update(entity);
                await _context.SaveChangesAsync(tokenSource.Token);

                MessageBox.Show("Рецепт успешно обновлен");

                // Обновляем список
                ListBoxUI.ItemsSource = await GetAllAsync();
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

        private async Task<IList<Recipe>> GetAllAsync()
        {
            try
            {
                using var tokenSource = new CancellationTokenSource(
                    ConstansOperations.TokenTimeLimit);

                var entities = await _context.Recipes
                    .AsNoTracking()
                    .Include(recipe => recipe.Patient)
                    .Include(recipe => recipe.Doctor)
                    .Where(recipe =>
                        recipe.Patient.Id == PatientProfile.Profile.Id)
                    .ToListAsync(tokenSource.Token);

                return entities;
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Лимит времени");
                return new List<Recipe>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                return new List<Recipe>();
            }
        }
    }
}
