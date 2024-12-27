using armHospital.Data.Repositories;
using armHospital.Models;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Логика взаимодействия для MainAppWindow.xaml
    /// </summary>
    public partial class MainAppWindow : Window
    {
        private readonly AppointmentRepository _appointmentRepository;
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
