using Microsoft.AspNetCore.Identity;

namespace HospitalAppointmentSystem.Models
{
    public class Patient : IdentityUser
    {
        public string Name { get; set; }

        public string Phone { get; set; }
    }
}
