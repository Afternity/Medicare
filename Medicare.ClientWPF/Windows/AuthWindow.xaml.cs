using Medicare.ClientWPF.Consts;
using Medicare.ClientWPF.Profile;
using Medicare.Domain.Data.DbContexts;
using Medicare.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Medicare.ClientWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        private readonly MedicareDbContext _context;
        private string _login = "darina@gmail.com";
        private string _password = "5";

        public AuthWindow()
        {
            _context = new MedicareDbContext();
            InitializeComponent();
            loginTextBlock.Text = _login;
            passworkTextBlock.Text = _password;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _login = loginTextBlock.Text;
            _password = passworkTextBlock.Text;
        }

        private async void Auth_Click(object sender, RoutedEventArgs e)
        {
            _login = loginTextBlock.Text;
            _password = passworkTextBlock.Text;

            await AuthAsync(
                _login,
                _password);
        }

        private async Task AuthAsync(
            string login,
            string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(login)
                    || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Заполните поля");
                    return;
                }

                var tokenSource = new CancellationTokenSource(
                    ConstansOperations.TokenTimeLimit);

                var profile = await _context.Doctors
                    .Include(doctor => doctor.DoctorType)
                    .FirstOrDefaultAsync(profile =>
                        profile.Password == password
                        && profile.Email == login,
                        tokenSource.Token);

                if (profile == null)
                {
                    MessageBox.Show("Ошибка загрузки профиля из БД");
                    return;
                }

                DoctorProfile.Profile = profile;

                //MessageBox.Show("Вход успешен");

                await Task.Delay(1000);

                MainWindow();
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Лимин времени");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void MainWindow()
        {
            var window = new MainWindow();
            window.Show();
            this.Close();
        }

    }
}
