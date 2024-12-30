using armHospital.Models;
using armHospital.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            if (string.IsNullOrWhiteSpace(txtNewUsername.Text))
            {
                MessageBox.Show("Введите логин!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNewPassword.Password))
            {
                MessageBox.Show("Введите пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (txtNewPassword.Password != txtConfirmPassword.Password)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text) || !IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Введите корректный email!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPhoneNumber.Text) || !IsValidPhoneNumber(txtPhoneNumber.Text))
            {
                MessageBox.Show("Введите корректный номер телефона!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (cmbRole.SelectedItem == null)
            {
                MessageBox.Show("Выберите роль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Введите полное имя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var roleMap = new Dictionary<string, string>
            {
                 { "Пользователь", "user" },
                 { "Врач", "doctor" },
                 { "Администратор", "admin" }
            };

            var user = new User
            {
                Username = txtNewUsername.Text,
                Email = txtEmail.Text,
                Password = txtNewPassword.Password,
                PhoneNumber = txtPhoneNumber.Text,
                Role = roleMap[(cmbRole.SelectedItem as ComboBoxItem).Content.ToString()],
                FullName = txtFullName.Text
            };

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

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            string cleanedNumber = phoneNumber.Replace("+7", "").Replace(" ", "").Replace("-", "");
            return cleanedNumber.Length == 10;
        }

        private void txtPhoneNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
                return;
            }


            var textBox = sender as TextBox;
            string text = textBox.Text;

            if (text.Length == 0)
            {
                textBox.Text = "+7 ";
                textBox.CaretIndex = textBox.Text.Length;
            }
            else if (text.Length == 6)
            {
                textBox.Text += " ";
                textBox.CaretIndex = textBox.Text.Length;
            }
            else if (text.Length == 10)
            {
                textBox.Text += "-";
                textBox.CaretIndex = textBox.Text.Length;
            }
            else if (text.Length == 13)
            {
                textBox.Text += "-";
                textBox.CaretIndex = textBox.Text.Length;
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
