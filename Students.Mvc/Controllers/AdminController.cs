using Microsoft.AspNetCore.Mvc;

namespace Students.Mvc.Controllers;

public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    // Simple data endpoint returning aggregated sample data for charts.
    // Replace with real aggregated queries against your DB in production.
    [HttpGet]
    public IActionResult Data()
    {
        var gradesLabels = new[] { "Math", "Science", "English", "History", "Arts" };
        var gradesAverages = new[] { 78, 85, 72, 90, 81 };

        var distLabels = new[] { "A", "B", "C", "D", "F" };
        var distCounts = new[] { 25, 40, 20, 10, 5 };

        return Json(new
        {
            grades = new { labels = gradesLabels, data = gradesAverages },
            distribution = new { labels = distLabels, data = distCounts }
        });
    }
}
