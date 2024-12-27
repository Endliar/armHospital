using armHospital.Models;
using armHospital.Services;
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

namespace armHospital
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        private readonly AuthService _authService;
        public RegistrationWindow()
        {
            InitializeComponent();
            var context = new Data.DatabaseContext("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=0611;");
            var userRepository = new Data.Repositories.UserRepository(context);
            _authService = new Services.AuthService(userRepository);
        }

        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            var user = new User
            {
                Username = txtNewUsername.Text,
                Email = "sophia@example.com",
                Password = txtNewPassword.Password,
                PhoneNumber = "79137777713",
                Role = "doctor",
                FullName = "София Воронич"
            };

            if (txtNewPassword.Password == txtConfirmPassword.Password)
            {
                bool isRegistered = await _authService.Register(user);
                if (isRegistered)
                {
                    MessageBox.Show("Регистрация прошла успешно!");
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!");
                }
            }
            else
            {
                MessageBox.Show("Пароли не совпадают!");
            }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
