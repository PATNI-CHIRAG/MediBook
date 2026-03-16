using System.ComponentModel.DataAnnotations;

namespace HospitalAppointmentSystem.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentID { get; set; }
        public int DoctorID { get; set; }
        public string PatientID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentTime { get; set; }
        public string Status { get; set; } = "Pending";
        public Doctor Doctor { get; set; }
        public string? CancelledBy { get; set; }
    }
}