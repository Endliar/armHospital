using armHospital.Data.Repositories;
using armHospital.Models;
using System.Windows;
using System.Windows.Controls;

namespace armHospital
{
    /// <summary>
    /// Логика взаимодействия для MainAppWindow.xaml
    /// </summary>
    public partial class MainAppWindow : Window
    {
        private readonly AppointmentRepository _appointmentRepository;
        private readonly UserRepository _userRepository;
        private readonly int _userId;
        public string Role { get; set; } // Свойство для роли

        public MainAppWindow(int userId, string role)
        {
            InitializeComponent();
            _userId = userId;
            Role = role;
            DataContext = this;

            var context = new Data.DatabaseContext("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=0611;");
            _appointmentRepository = new AppointmentRepository(context);
            _userRepository = new UserRepository(context);

            btnAddAppointment.Click += BtnAddAppointment_Click;

            LoadAppointments(userId, role);
            LoadProfile(userId);

            tabControl.SelectionChanged += TabControl_SelectionChanged;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.SelectedItem is TabItem selectedTab && selectedTab.Header.ToString() == "Мои записи")
            {
                LoadUserAppointments(_userId);
            }
        }

        private async void LoadAppointments(int userId, string role)
        {
            List<Appointment> appointments;

            if (role == "doctor")
            {
                appointments = await _appointmentRepository.GetAppointmentsByDoctorId(userId);
            }
            else if (role == "user")
            {
                appointments = await _appointmentRepository.GetScheduledAppointments();
                appointments = appointments.Where(a => !a.ClientId.HasValue).ToList();
                lvAppointments.ItemsSource = appointments;
            }
            else
            {
                appointments = await _appointmentRepository.GetAllAppointments();
            }

            if (appointments == null || appointments.Count == 0)
            {
                txtNoAppointments.Visibility = Visibility.Visible;
            }
            else
            {
                txtNoAppointments.Visibility = Visibility.Collapsed;

                foreach (var appointment in appointments)
                {
                    if (appointment.DoctorId.HasValue)
                    {
                        var doctor = await _userRepository.GetUserById(appointment.DoctorId.Value);
                        if (doctor != null)
                        {
                            appointment.DoctorFullName = doctor.FullName;
                        }
                    }
                }

                lvAppointments.ItemsSource = appointments;
            }

            if (role == "admin")
            {
                btnAddAppointment.Visibility = Visibility.Visible;
            }
        }

        private async void LoadProfile(int userId)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user != null)
            {
                txtEmail.Text = user.Email;
                txtPhoneNumber.Text = user.PhoneNumber;
                txtRole.Text = user.Role;
                txtFullName.Text = user.FullName;
            }
        }

        private async void LoadUserAppointments(int userId)
        {
            var appointments = await _appointmentRepository.GetAllAppointments();

            appointments = appointments.Where(a => a.ClientId == userId).ToList();

            if (appointments == null || appointments.Count == 0)
            {
                txtNoMyAppointments.Visibility = Visibility.Visible;
            }
            else
            {
                txtNoMyAppointments.Visibility = Visibility.Collapsed;

                foreach (var appointment in appointments)
                {
                    if (appointment.DoctorId.HasValue)
                    {
                        var doctor = await _userRepository.GetUserById(appointment.DoctorId.Value);
                        if (doctor != null)
                        {
                            appointment.DoctorFullName = doctor.FullName;
                        }
                    }
                }

                lvMyAppointments.ItemsSource = appointments;
            }
        }

        private void BtnAddAppointment_Click(object sender, RoutedEventArgs e)
        {
            var createAppointmentWindow = new CreateAppointmentWindow(_userId, Role);
            createAppointmentWindow.ShowDialog();
            LoadAppointments(_userId, Role);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Appointment appointment)
            {
                var editWindow = new CreateAppointmentWindow(_userId, appointment, Role);
                editWindow.ShowDialog();
                LoadAppointments(_userId, Role);
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Appointment appointment)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления",
                                             MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _appointmentRepository.DeleteAppointment(appointment.Id);
                        MessageBox.Show("Запись успешно удалена!");

                        LoadAppointments(_userId, Role);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении записи: {ex.Message}", "Ошибка",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void btnEditProfile_Click(object sender, RoutedEventArgs e)
        {
            txtEmail.IsReadOnly = false;
            txtPhoneNumber.IsReadOnly = false;
            txtFullName.IsReadOnly = false;

            btnEditProfile.Visibility = Visibility.Collapsed;
            btnSaveProfile.Visibility = Visibility.Visible;
        }

        private async void btnSaveProfile_Click(object sender, RoutedEventArgs e)
        {
            // Валидация полей
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

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Введите полное имя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Обновляем данные пользователя
            var user = await _userRepository.GetUserById(_userId);
            if (user != null)
            {
                user.Email = txtEmail.Text;
                user.PhoneNumber = txtPhoneNumber.Text;
                user.FullName = txtFullName.Text;

                await _userRepository.UpdateUser(user);
                MessageBox.Show("Данные профиля успешно обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            // Возвращаемся в режим просмотра
            txtEmail.IsReadOnly = true;
            txtPhoneNumber.IsReadOnly = true;
            txtFullName.IsReadOnly = true;

            btnSaveProfile.Visibility = Visibility.Collapsed;
            btnEditProfile.Visibility = Visibility.Visible;
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

        // Валидация номера телефона (простой пример)
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return !string.IsNullOrWhiteSpace(phoneNumber) && phoneNumber.Length >= 10;
        }

        private async void btnBook_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Appointment appointment)
            {
                try
                {
                    appointment.ClientId = _userId;

                    await _appointmentRepository.UpdateAppointment(appointment);

                    MessageBox.Show("Вы успешно записаны на прием!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Обновляем данные на обеих вкладках
                    LoadAppointments(_userId, Role);
                    LoadUserAppointments(_userId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при записи на прием: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
