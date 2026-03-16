using HospitalAppointmentSystem.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

public class DoctorAccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public DoctorAccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Login()
    {

        return View();
    }

    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        HttpContext.SignOutAsync();

        var doctor = _context.Doctors
            .FirstOrDefault(d => d.Email == email && d.Password == password);

        if (doctor != null)
        {
            HttpContext.Session.SetInt32("DoctorID", doctor.DoctorID);

            TempData["Success"] = "Doctor login successful!";

            return RedirectToAction("Dashboard", "Doctors");
        }

        TempData["Error"] = "Invalid email or password";
        return RedirectToAction("Login");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove("DoctorID");

        TempData["Success"] = "Logout successful";

        return RedirectToAction("Login");
    }
}