namespace HospitalAppointmentSystem.Models
{
    public class Doctor
    {
        public int DoctorID { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Specialization { get; set; }

        public string? Experience { get; set; }

        public string? About { get; set; }

        public decimal? AppointmentFee { get; set; }

        public string? Status { get; set; }

        public string? ImageUrl { get; set; }
    }
}