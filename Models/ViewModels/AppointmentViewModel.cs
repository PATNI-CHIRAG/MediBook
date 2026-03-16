namespace HospitalAppointmentSystem.Models.ViewModels
{
    public class AppointmentViewModel
    {
        public int AppointmentID { get; set; }

        public string DoctorName { get; set; }

        public string Specialization { get; set; }

        public string ImageUrl { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string AppointmentTime { get; set; }

        public string Status { get; set; }
    }
}