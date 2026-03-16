using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HospitalAppointmentSystem.Models;

public class AccountController : Controller
{
    private readonly UserManager<Patient> _userManager;
    private readonly SignInManager<Patient> _signInManager;

    public AccountController(UserManager<Patient> userManager,
                             SignInManager<Patient> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(Patient model, string password)
    {
        if (ModelState.IsValid)
        {
            model.UserName = model.Email;
            HttpContext.Session.Remove("DoctorID");

            var result = await _userManager.CreateAsync(model, password);

            if (result.Succeeded)
            {
                TempData["Success"] = "Registration successful! Please login.";
                return RedirectToAction("Login", "Account");
            }
        }

        TempData["Error"] = "Registration failed!";
        return RedirectToAction("Register", "Account");
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {

        HttpContext.Session.Remove("DoctorID");
        HttpContext.Session.Remove("Admin");

        var result = await _signInManager.PasswordSignInAsync(email, password, false, false);

        if (result.Succeeded)
        {
            TempData["Success"] = "Login successful!";
            return RedirectToAction("Index", "Home");
        }

        TempData["Error"] = "Login failed!";
        return RedirectToAction("Login", "Account");
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        TempData["Success"] = "Logout successful!";
        return RedirectToAction("Index", "Home");
    }
}