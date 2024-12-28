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
        }

        private async void LoadAppointments(int userId, string role)
        {
            List<Appointment> appointments;

            if (role == "doctor")
            {
                appointments = await _appointmentRepository.GetAppointmentsByDoctorId(userId);
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

        private void BtnAddAppointment_Click(object sender, RoutedEventArgs e)
        {
            var createAppointmentWindow = new CreateAppointmentWindow(_userId);
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
    }
}
