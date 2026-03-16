using HospitalAppointmentSystem.Data;
using HospitalAppointmentSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAppointmentSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ADMIN LOGIN PAGE
        public IActionResult Login()
        {
            return View();
        }

        // ADMIN LOGIN CHECK
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // clear other logins
            //HttpContext.Session.Remove("DoctorID");
            await HttpContext.SignOutAsync(); // logout patient

            if (email == "admin@gmail.com" && password == "admin")
            {
                HttpContext.Session.SetString("Admin", "true");

                TempData["Success"] = "Admin Login Successful";
                return RedirectToAction("Dashboard");
            }

            TempData["Error"] = "Invalid Admin Credentials";
            return RedirectToAction("Login");
        }

        // ADMIN DASHBOARD
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("Admin") == null)
            {
                return RedirectToAction("Login");
            }

            var doctors = _context.Doctors.ToList();
            return View(doctors);
        }

        // LOGOUT
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Admin");
            TempData["Success"] = "Admin Logout Successful";
            return RedirectToAction("Login");
        }

        public IActionResult CreateDoctor()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateDoctor(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            _context.SaveChanges();

            TempData["Success"] = "Doctor Added Successfully";

            return RedirectToAction("Dashboard");
        }


        public IActionResult EditDoctor(int id)
        {
            var doctor = _context.Doctors.Find(id);
            return View(doctor);
        }

        [HttpPost]
        public IActionResult EditDoctor(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
            _context.SaveChanges();

            TempData["Success"] = "Doctor Updated Successfully";

            return RedirectToAction("Dashboard");
        }


        public IActionResult DeleteDoctor(int id)
        {
            var doctor = _context.Doctors.Find(id);

            if (doctor != null)
            {
                _context.Doctors.Remove(doctor);
                _context.SaveChanges();
            }

            TempData["Success"] = "Doctor Deleted Successfully";

            return RedirectToAction("Dashboard");
        }



    }

   
}