namespace armHospital.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? DoctorId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string DoctorFullName { get; set; }
        public int? ClientId { get; set; }
    }
}
