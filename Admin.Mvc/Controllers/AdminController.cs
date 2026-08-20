using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Admin.Mvc.Data;
using Admin.Mvc.Models;

namespace Admin.Mvc.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly StudentsDbContext _studentsContext;
        private readonly GradesDbContext _gradesContext;

        public AdminController(StudentsDbContext studentsContext, GradesDbContext gradesContext)
        {
            _studentsContext = studentsContext;
            _gradesContext = gradesContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Aggregated data for dashboard
        [HttpGet]
        public async Task<IActionResult> Data()
        {
            // Average by course
            var averages = await _gradesContext.Grades
                .GroupBy(g => g.CourseName)
                .Select(g => new { Course = g.Key, Avg = (int)g.Average(x => x.Score) })
                .ToListAsync();

            var gradesLabels = averages.Select(a => a.Course).ToArray();
            var gradesAverages = averages.Select(a => a.Avg).ToArray();

            // Distribution by grade band
            var distribution = await _gradesContext.Grades
                .Select(g => new { g.Score })
                .ToListAsync();

            int aCount = distribution.Count(d => d.Score >= 90);
            int bCount = distribution.Count(d => d.Score >= 80 && d.Score < 90);
            int cCount = distribution.Count(d => d.Score >= 70 && d.Score < 80);
            int dCount = distribution.Count(d => d.Score >= 60 && d.Score < 70);
            int fCount = distribution.Count(d => d.Score < 60);

            var distLabels = new[] { "A", "B", "C", "D", "F" };
            var distCounts = new[] { aCount, bCount, cCount, dCount, fCount };

            return Json(new
            {
                grades = new { labels = gradesLabels, data = gradesAverages },
                distribution = new { labels = distLabels, data = distCounts }
            });
        }

        // Return students list for selection
        [HttpGet]
        public async Task<IActionResult> Students()
        {
            var list = await _studentsContext.Students
                .Select(s => new { s.Id, Name = s.FirstName + " " + s.LastName })
                .OrderBy(s => s.Name)
                .ToListAsync();

            return Json(list);
        }

        // Return marks for a specific student
        [HttpGet]
        public async Task<IActionResult> StudentMarks(int id)
        {
            var marks = await _gradesContext.Grades
                .Where(g => g.StudentId == id)
                .Select(g => new { g.CourseName, g.Score, g.GradeDate })
                .OrderByDescending(g => g.GradeDate)
                .ToListAsync();

            return Json(marks);
        }
    }
}
