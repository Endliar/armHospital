using armHospital.Data.Repositories;
using armHospital.Models;
using System.Windows;
using System.Windows.Controls;

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
        private readonly Appointment _existingAppointment;

        public bool IsAdmin { get; set; }

        public CreateAppointmentWindow(int adminUserId, string role)
        {
            InitializeComponent();
            _adminUserId = adminUserId;
            IsAdmin = role == "admin";
            DataContext = this;

            var context = new Data.DatabaseContext("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=0611;");
            _appointmentRepository = new AppointmentRepository(context);
            _userRepository = new UserRepository(context);

            LoadDoctors();
            LoadStatuses();
        }

        public CreateAppointmentWindow(int adminUserId, Appointment appointment, string role)
        {
            InitializeComponent();
            _adminUserId = adminUserId;
            _existingAppointment = appointment;
            IsAdmin = role == "admin";
            DataContext = this;

            var context = new Data.DatabaseContext("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=0611;");
            _appointmentRepository = new AppointmentRepository(context);
            _userRepository = new UserRepository(context);

            LoadDoctors();
            LoadStatuses();
            LoadAppointmentData();
        }

        private async void LoadDoctors()
        {
            var doctors = await _userRepository.GetUsersByRole("doctor");
            cmbDoctors.ItemsSource = doctors;
        }

        private void LoadAppointmentData()
        {
            if (_existingAppointment != null)
            {
                cmbDoctors.SelectedValue = _existingAppointment.DoctorId;
                txtTitle.Text = _existingAppointment.Title;
                txtDescription.Text = _existingAppointment.Description;
                dpAppointmentDate.SelectedDate = _existingAppointment.AppointmentDate.Date;
                txtAppointmentTime.Text = _existingAppointment.AppointmentDate.ToString("HH:mm");
                txtComments.Text = _existingAppointment.Comments;
                cmbStatus.SelectedValue = _existingAppointment.Status;
            }
        }

        private void LoadStatuses()
        {
            var statuses = new Dictionary<string, string>
            {
                { "pending", "Ожидается" },
                { "confirmed", "Подтверждено" },
                { "cancelled", "Отменено" },
                { "completed", "Завершено" }
            };

            cmbStatus.ItemsSource = statuses;
            cmbStatus.SelectedValuePath = "Key";
            cmbStatus.DisplayMemberPath = "Value";

            if (_existingAppointment != null)
            {
                cmbStatus.SelectedValue = _existingAppointment.Status;
            }
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

            if (cmbStatus.SelectedValue == null)
            {
                cmbStatusError.Text = "Выберите статус!";
                cmbStatusError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                cmbStatusError.Visibility = Visibility.Collapsed;
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
                Id = _existingAppointment?.Id ?? 0,
                Title = txtTitle.Text,
                Description = txtDescription.Text,
                AppointmentDate = dpAppointmentDate.SelectedDate.Value.Date + TimeSpan.Parse(txtAppointmentTime.Text),
                Comments = txtComments.Text,
                Status = cmbStatus.SelectedValue.ToString(),
                DoctorId = (int)cmbDoctors.SelectedValue,
                UserId = _adminUserId
            };

            if (_existingAppointment == null)
            {
                await _appointmentRepository.AddAppointment(appointment);
                MessageBox.Show("Запись успешно создана!");
            }
            else
            {
                await _appointmentRepository.UpdateAppointment(appointment);
                MessageBox.Show("Запись успешно обновлена!");
            }

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
