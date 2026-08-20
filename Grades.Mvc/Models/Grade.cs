namespace Grades.Mvc.Models;
    public class Grade
    {
       public int Id { get; set; }

       public int StudentId { get; set; }   

       public string CourseName { get; set; }

       public int Score { get; set; }

       public DateTime GradeDate { get; set; }

       public string? Notes { get; set; }

       // Teacher who graded this
       public int? TeacherId { get; set; }
    }

