namespace HospitalAppointmentSystem.Models.ViewModels
{
    public class DoctorAppointmentViewModel
    {
        public int AppointmentID { get; set; }

        public string PatientName { get; set; }
        public string PatientEmail { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string AppointmentTime { get; set; }

        public string Status { get; set; }
    }
}