namespace Students.Mvc.Models;

public class Teacher
{
    public int Id { get; set; }
    public string? ApplicationUserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
