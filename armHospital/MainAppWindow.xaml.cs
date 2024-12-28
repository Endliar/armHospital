using armHospital.Data.Repositories;
using System.Windows;

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

            LoadAppointments(userId, role);
        }

        private async void LoadAppointments(int userId, string role)
        {
            var appointments = await _appointmentRepository.GetAllAppointments();
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
                btnAddAppointment.Click += BtnAddAppointment_Click;
            }
        }

        private void BtnAddAppointment_Click(object sender, RoutedEventArgs e)
        {
            var createAppointmentWindow = new CreateAppointmentWindow(_userId);
            createAppointmentWindow.ShowDialog();
            LoadAppointments(_userId, Role);
        }
    }
}
