using Grades.Mvc.DTOs;

namespace Grades.Mvc.Services
{
    public class StudentsServiceClient
    {
        private readonly HttpClient _http;

        public StudentsServiceClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<StudentDto>> GetAllStudents()
        {
            return await _http.GetFromJsonAsync<List<StudentDto>>("Students/GetAll");
        }

        public async Task<StudentDto?> GetById(int id)
        {
            return await _http.GetFromJsonAsync<StudentDto>($"Students/{id}");
        }

        public async Task<List<dynamic>> GetAllTeachers()
        {
            return await _http.GetFromJsonAsync<List<dynamic>>("Teachers/GetAll");
        }
    }
}
