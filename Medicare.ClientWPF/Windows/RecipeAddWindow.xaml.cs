using Medicare.ClientWPF.Consts;
using Medicare.ClientWPF.Profile;
using Medicare.Domain.Data.DbContexts;
using Medicare.Domain.Models;
using System.Windows;

namespace Medicare.ClientWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для RecipeAddWindow.xaml
    /// </summary>
    public partial class RecipeAddWindow 
        : Window
    {
        private readonly MedicareDbContext _context;
        private readonly Patient _currentPatient;
        private readonly Doctor _currentDoctor;

        public RecipeAddWindow()
        {
            InitializeComponent();
            _context = new MedicareDbContext();
            _currentPatient = PatientProfile.Profile; // Пациент из профиля
            _currentDoctor = DoctorProfile.Profile;   // Авторизованный врач
            Loaded += RecipeAddWindow_Loaded;
        }

        private void RecipeAddWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Заполняем информацию о пациенте и враче
            PatientTextBlock.Text = _currentPatient.FullName;
            DoctorTextBlock.Text = _currentDoctor.FullName;
        }

        private async void AddButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                var recipe = new Recipe
                {
                    Id = Guid.NewGuid(),
                    Medication = MedicationTextBox.Text.Trim(),
                    Dosage = DosageTextBox.Text.Trim(),
                    Instructions = InstructionsTextBox.Text.Trim(),
                    DurationDays = TimeSpan.FromDays(int.Parse(DurationDaysTextBox.Text)),
                    IssueDate = DateTime.Now,
                    PatientId = _currentPatient.Id,    // ID из профиля пациента
                    DoctorId = _currentDoctor.Id       // ID авторизованного врача
                };

                await SaveRecipeAsync(recipe);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании рецепта: {ex.Message}");
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(MedicationTextBox.Text))
            {
                MessageBox.Show("Введите название лекарства");
                return false;
            }

            if (string.IsNullOrWhiteSpace(DosageTextBox.Text))
            {
                MessageBox.Show("Введите дозировку");
                return false;
            }

            if (!int.TryParse(DurationDaysTextBox.Text, out int duration) || duration <= 0)
            {
                MessageBox.Show("Введите корректную длительность приема (положительное число)");
                return false;
            }

            return true;
        }

        private async Task SaveRecipeAsync(
            Recipe recipe)
        {
            try
            {
                using var tokenSource = new CancellationTokenSource(
                    ConstansOperations.TokenTimeLimit);

                await _context.Recipes.AddAsync(recipe, tokenSource.Token);
                await _context.SaveChangesAsync(tokenSource.Token);

                MessageBox.Show("Рецепт успешно выписан");
                this.Close();
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Лимит времени при сохранении");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void CancelButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

