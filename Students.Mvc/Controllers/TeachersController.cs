using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Students.Mvc.Data;
using Students.Mvc.Models;

namespace Students.Mvc.Controllers;

[Authorize(Roles = "Admin")]
public class TeachersController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public TeachersController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        var list = _db.Teachers.ToList();
        return View(list);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(string firstName, string lastName, string email, string password)
    {
        // create identity user
        var user = new ApplicationUser { UserName = email, Email = email };
        var res = await _userManager.CreateAsync(user, password);
        if (!res.Succeeded)
        {
            foreach (var e in res.Errors) ModelState.AddModelError(string.Empty, e.Description);
            return View();
        }
        await _userManager.AddToRoleAsync(user, "Teacher");

        var teacher = new Teacher { ApplicationUserId = user.Id, FirstName = firstName, LastName = lastName, Email = email };
        _db.Teachers.Add(teacher);
        await _db.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    // API for other services to get teachers
    [AllowAnonymous]
    [HttpGet]
    public IActionResult GetAll()
    {
        var list = _db.Teachers.Select(t => new { t.Id, Name = t.FirstName + " " + t.LastName }).ToList();
        return Json(list);
    }
}
