using HospitalAppointmentSystem.Data;
using HospitalAppointmentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using HospitalAppointmentSystem.Models.ViewModels;

public class DoctorsController : Controller
{
    private readonly ApplicationDbContext _context;

    public DoctorsController(ApplicationDbContext context)
    {
        _context = context;
    }


    public IActionResult Dashboard()
    {
        var doctorId = HttpContext.Session.GetInt32("DoctorID");

        if (doctorId == null)
            return RedirectToAction("Login", "DoctorAccount");

        var appointments = _context.Appointments
            .Where(a => a.DoctorID == doctorId &&
                   (a.CancelledBy == null || a.CancelledBy != "Patient"))
            .OrderByDescending(a => a.AppointmentID)
            .Join(_context.Users,
                a => a.PatientID,
                u => u.Id,
                (a, u) => new DoctorAppointmentViewModel
                {
                    AppointmentID = a.AppointmentID,
                    PatientName = u.Name,
                    PatientEmail = u.Email,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    Status = a.Status
                })
            .ToList();

        return View(appointments);
    }


    public IActionResult Index(string specialization)
    {
        var doctors = _context.Doctors.AsQueryable();

        if (!string.IsNullOrEmpty(specialization))
        {
            doctors = doctors.Where(d => d.Specialization == specialization);
        }

        return View(doctors.ToList());
    }

    public IActionResult Details(int id, DateTime? selectedDate)
    {
        var doctor = _context.Doctors.FirstOrDefault(d => d.DoctorID == id);

        if (doctor == null)
            return NotFound();

        if (doctor.Status == "Unavailable")
        {
            TempData["Error"] = $"{doctor.Name} is not available.";
            return RedirectToAction("Index");
        }

        // generate next 7 days
        var dates = new List<DateTime>();

        for (int i = 1; i < 8; i++)
        {
            dates.Add(DateTime.Today.AddDays(i));
        }

        ViewBag.Dates = dates;
        ViewBag.SelectedDate = selectedDate;

        // Time slots
        ViewBag.Times = new List<string>
    {
        "10:00 AM",
        "10:30 AM",
        "11:00 AM",
        "11:30 AM",
        "2:00 PM",
        "2:30 PM",
        
    };

        var bookedTimes = new List<string>();

        if (selectedDate != null)
        {
            bookedTimes = _context.Appointments
                .Where(a => a.DoctorID == id &&
                            a.AppointmentDate.Date == selectedDate.Value.Date &&
                            a.Status != "Cancelled")
                .Select(a => a.AppointmentTime)
                .ToList();
        }

        ViewBag.BookedTimes = bookedTimes;

        return View(doctor);
    }

    [Authorize]
[HttpPost]
public IActionResult Book(int doctorId, DateTime? date, string time)
{
        // Check if doctor is logged in
        var doctorSession = HttpContext.Session.GetInt32("DoctorID");
        var adminSession = HttpContext.Session.GetString("Admin");

        if (doctorSession != null || adminSession != null)
        {
            TempData["Error"] = "Only patients can book appointments.";
            return RedirectToAction("Index");
        }

        if (date == null || string.IsNullOrEmpty(time))
    {
        TempData["Error"] = "Please select date and time before booking.";
        return RedirectToAction("Details", new { id = doctorId, selectedDate = date });
    }

    var exists = _context.Appointments.Any(a =>
        a.DoctorID == doctorId &&
        a.AppointmentDate.Date == date.Value.Date &&
        a.AppointmentTime == time);

    if (exists)
    {
        TempData["Error"] = "This slot is already booked.";
        return RedirectToAction("Details", new { id = doctorId, selectedDate = date });
    }

    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    var appointment = new Appointment
    {
        DoctorID = doctorId,
        PatientID = userId,
        AppointmentDate = date.Value,
        AppointmentTime = time,
        Status = "Pending"
    };

    _context.Appointments.Add(appointment);
    _context.SaveChanges();

    TempData["Success"] = "Appointment booked successfully.";

    return RedirectToAction("MyAppointments");
}

    [Authorize]
    public IActionResult MyAppointments()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var appointments = _context.Appointments
            .Where(a => a.PatientID == userId)
            .OrderByDescending(a => a.AppointmentID) // latest booked first
            .Join(_context.Doctors,
                a => a.DoctorID,
                d => d.DoctorID,
                (a, d) => new AppointmentViewModel
                {
                    AppointmentID = a.AppointmentID,
                    DoctorName = d.Name,
                    Specialization = d.Specialization,
                    ImageUrl = d.ImageUrl,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    Status = a.Status
                })
            .ToList();

        return View(appointments);
    }

    [Authorize]
    public IActionResult CancelAppointment(int id)
    {
        var appointment = _context.Appointments
            .FirstOrDefault(a => a.AppointmentID == id);

        if (appointment == null)
        {
            return RedirectToAction("MyAppointments");
        }

        if (appointment.Status == "Pending" || appointment.Status == "Accepted")
        {
            appointment.Status = "Cancelled";
            appointment.CancelledBy = "Patient";

            _context.SaveChanges();
        }

        return RedirectToAction("MyAppointments");
    }

    public IActionResult Accept(int id)
    {
        var appt = _context.Appointments.Find(id);

        if (appt == null) return RedirectToAction("Dashboard");

        appt.Status = "Accepted";

        _context.SaveChanges();

        return RedirectToAction("Dashboard");
    }

    public IActionResult Reject(int id)
    {
        var appt = _context.Appointments.Find(id);

        if (appt == null) return RedirectToAction("Dashboard");

        appt.Status = "Cancelled";
        appt.CancelledBy = "Doctor";

        _context.SaveChanges();

        return RedirectToAction("Dashboard");
    }

    public IActionResult Done(int id)
    {
        var appt = _context.Appointments.Find(id);

        if (appt == null) return RedirectToAction("Dashboard");

        appt.Status = "Done";

        _context.SaveChanges();

        return RedirectToAction("Dashboard");
    }


}