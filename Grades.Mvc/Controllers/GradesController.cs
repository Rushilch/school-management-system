using Grades.Mvc.Data;
using Grades.Mvc.DTOs;
using Grades.Mvc.Models;
using Grades.Mvc.Services;
using Grades.Mvc.Services;
using Grades.Mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grades.Mvc.Controllers
{
    public class GradesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly StudentsServiceClient _studentsClient;

        public GradesController(ApplicationDbContext context , StudentsServiceClient studentsClient)
        {
            _context = context;
            _studentsClient = studentsClient;
        }

        // GET: Grades
        public async Task<IActionResult> Index()
        {
            var grades = await _context.Grades.ToListAsync();

            List<StudentDto> students = new();

            try
            {
                students = await _studentsClient.GetAllStudents();
            }
            catch
            {
                
                students = new List<StudentDto>();
            }

            var result = grades.Select(g =>
            {
                var student = students.FirstOrDefault(s => s.Id == g.StudentId);

                return new GradeViewModel
                {
                    Id = g.Id,
                    CourseName = g.CourseName,
                    Score = g.Score,
                    GradeDate = g.GradeDate,
                    StudentName = student == null
                        ? "Unknown"
                        : $"{student.FirstName} {student.LastName}"
                };
            });

            return View(result);
        }

        // GET: Grades/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var grade = await _context.Grades.FirstOrDefaultAsync(x => x.Id == id);
            if (grade == null) return NotFound();

            var students = await _studentsClient.GetAllStudents();
            var student = students.FirstOrDefault(s => s.Id == grade.StudentId);

            var vm = new GradeViewModel
            {
                Id = grade.Id,
                CourseName = grade.CourseName,
                Score = grade.Score,
                GradeDate = grade.GradeDate,
                StudentName = student == null ? "Unknown" : $"{student.FirstName} {student.LastName}"
            };

            return View(vm);
        }

        // GET: Grades/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var students = await _studentsClient.GetAllStudents();

                ViewBag.Students = students;

                // Try to fetch teachers from Students service
                try
                {
                    var teachers = await _studentsClient.GetAllTeachers();
                    ViewBag.Teachers = teachers;
                }
                catch
                {
                    ViewBag.Teachers = new List<dynamic>();
                }

                return View();
            }
            catch
            {
                ViewBag.Error = "Students service is unavailable.";

                return View();
            }
        }

        // POST: Grades/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentId,CourseName,Score,GradeDate,Notes,TeacherId")] Grade grade)
        {
            if (ModelState.IsValid)
            {
                _context.Add(grade);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(grade);
        }

        // GET: Grades/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var grade = await _context.Grades.FindAsync(id);

            if (grade == null)
                return NotFound();

            var students = await _studentsClient.GetAllStudents();

            ViewBag.Students = students;

            try
            {
                var teachers = await _studentsClient.GetAllTeachers();
                ViewBag.Teachers = teachers;
            }
            catch
            {
                ViewBag.Teachers = new List<dynamic>();
            }

            return View(grade);
        }

        // POST: Grades/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentId,CourseName,Score,GradeDate,Notes")] Grade grade)
        {
            if (id != grade.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(grade);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GradeExists(grade.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(grade);
        }

        // GET: Grades/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var grade = await _context.Grades.FirstOrDefaultAsync(x => x.Id == id);

            var students = await _studentsClient.GetAllStudents();
            var student = students.FirstOrDefault(s => s.Id == grade.StudentId);

            var vm = new GradeViewModel
            {
                Id = grade.Id,
                CourseName = grade.CourseName,
                Score = grade.Score,
                GradeDate = grade.GradeDate,
                StudentName = student == null ? "Unknown" : $"{student.FirstName} {student.LastName}"
            };

            return View(vm);
        }

        // POST: Grades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Console.WriteLine($"POST DELETE ID = {id}");

            var grade = await _context.Grades.FindAsync(id);

            if (grade == null)
            {
                return NotFound();
            }

            _context.Grades.Remove(grade);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool GradeExists(int id)
        {
            return _context.Grades.Any(e => e.Id == id);
        }
    }
}
