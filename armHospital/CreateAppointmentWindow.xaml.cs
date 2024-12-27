using armHospital.Data.Repositories;
using armHospital.Models;
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
    /// Логика взаимодействия для CreateAppointmentWindow.xaml
    /// </summary>
    public partial class CreateAppointmentWindow : Window
    {
        private readonly AppointmentRepository _appointmentRepository;
        private readonly UserRepository _userRepository;
        private readonly int _adminUserId;

        public CreateAppointmentWindow(int adminUserId)
        {
            InitializeComponent();
            _adminUserId = adminUserId;
            var context = new Data.DatabaseContext("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=0611;");
            _appointmentRepository = new AppointmentRepository(context);
            _userRepository = new UserRepository(context);

            LoadDoctors();
        }

        private async void LoadDoctors()
        {
            var doctors = await _userRepository.GetUsersByRole("doctor");
            cmbDoctors.ItemsSource = doctors;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;

            if (cmbDoctors.SelectedValue == null)
            {
                cmbDoctorsError.Text = "Выберите доктора!";
                cmbDoctorsError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                cmbDoctorsError.Visibility = Visibility.Collapsed;
            }

            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                txtTitleError.Text = "Заголовок не может быть пустым!";
                txtTitleError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                txtTitleError.Visibility = Visibility.Collapsed;
            }

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                txtDescriptionError.Text = "Описание не может быть пустым!";
                txtDescriptionError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                txtDescriptionError.Visibility = Visibility.Collapsed;
            }

            if (dpAppointmentDate.SelectedDate == null)
            {
                dpAppointmentDateError.Text = "Выберите дату!";
                dpAppointmentDateError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                dpAppointmentDateError.Visibility = Visibility.Collapsed;
            }

            if (!TimeSpan.TryParse(txtAppointmentTime.Text, out _))
            {
                txtAppointmentTimeError.Text = "Введите время в формате HH:mm!";
                txtAppointmentTimeError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                txtAppointmentTimeError.Visibility = Visibility.Collapsed;
            }

            if (!isValid)
            {
                return;
            }

            var appointment = new Appointment
            {
                Title = txtTitle.Text,
                Description = txtDescription.Text,
                AppointmentDate = dpAppointmentDate.SelectedDate.Value.Date + TimeSpan.Parse(txtAppointmentTime.Text),
                Comments = txtComments.Text,
                Status = "pending", // По умолчанию статус "ожидание"
                DoctorId = (int)cmbDoctors.SelectedValue, // ID выбранного доктора
                UserId = _adminUserId // ID администратора, создающего запись
            };

            await _appointmentRepository.AddAppointment(appointment);
            MessageBox.Show("Запись успешно создана!");
            this.Close();
        }

        private void cmbDoctors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbDoctors.SelectedItem != null)
            {
                var selectedDoctor = cmbDoctors.SelectedItem as User;
                if (selectedDoctor != null)
                {
                    cmbDoctors.Text = selectedDoctor.FullName;
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
