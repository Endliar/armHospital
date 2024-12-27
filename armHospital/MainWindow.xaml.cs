using armHospital.Services;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace armHospital
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AuthService _authService;
        public MainWindow()
        {
            InitializeComponent();
            var context = new Data.DatabaseContext("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=0611;");
            var userRepository = new Data.Repositories.UserRepository(context);
            _authService = new Services.AuthService(userRepository);
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var user = await _authService.Login(txtUsername.Text, txtPassword.Password);
            if (user != null)
            {
                MessageBox.Show("Вход выполнен успешно!");
                var mainAppWindow = new MainAppWindow(user.Id, user.Role);
                mainAppWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!");
            }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow();
            registrationWindow.Show();
            this.Close(); 
        }
    }
}