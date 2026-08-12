using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using SchoolManagentSystem_API.DBContext;
using SchoolManagentSystem_API.Dtos.StudentDTO;
using SchoolManagentSystem_API.Dtos.TeacherDTO;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SchoolManagentSystem_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("student-list")]
        public async Task<ActionResult> GetAllStudentDetails(
    GetAllStudentListDetailsRequestDTO request)
        {
            var query =
                from student in _context.Students
                join cls in _context.Classes
                    on student.ClassID equals cls.ClassID
                join school in _context.Schools
                    on student.SchoolID equals school.SchoolID
                where student.IsActive && !student.IsDeleted
                select new
                {
                    Student = student,
                    ClassName = cls.ClassName,
                    SchoolName = school.SchoolName
                };

            // Search
            if (!string.IsNullOrWhiteSpace(request.SearchString))
            {
                var search = request.SearchString.Trim().ToLower();

                query = query.Where(x =>
                    x.Student.StudentName.ToLower().Contains(search) ||
                    x.Student.RollNumber.ToLower().Contains(search) ||
                    x.Student.Email.ToLower().Contains(search) ||
                    x.Student.FatherName.ToLower().Contains(search) ||
                    x.Student.MotherName.ToLower().Contains(search) ||
                    x.Student.FatherPhoneNumber.Contains(search) ||
                    x.Student.MotherPhoneNumber.Contains(search) ||
                    x.ClassName.ToLower().Contains(search) ||
                    x.SchoolName.ToLower().Contains(search));
            }

            // Total Count
            var totalCount = await query.CountAsync();

            // Page number & page size
            var pageNo = request.PageNo <= 0 ? 1 : request.PageNo;
            var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

            // Auto-correct page number
            if ((pageNo - 1) * pageSize >= totalCount && totalCount > 0)
            {
                pageNo = 1;
            }

            // Pagination + Projection
            var students = await query
                .OrderBy(x => x.Student.StudentId)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new GetAllStudentListDetailsResponseDTO
                {
                    StudentId = x.Student.StudentId,
                    StudentName = x.Student.StudentName,
                    RollNumber = x.Student.RollNumber,
                    Email = x.Student.Email,
                    Gender = x.Student.Gender,
                    StudentBirthDate = x.Student.StudentBirthDate,
                    BloodGroup = x.Student.BloodGroup,
                    Address = x.Student.Address,

                    FatherName = x.Student.FatherName,
                    MotherName = x.Student.MotherName,
                    FatherPhoneNumber = x.Student.FatherPhoneNumber,
                    MotherPhoneNumber = x.Student.MotherPhoneNumber,

                    SchoolID = x.Student.SchoolID,
                    SchoolName = x.SchoolName,

                    ClassID = x.Student.ClassID,
                    ClassName = x.ClassName
                })
                .ToListAsync();

            return Ok(new
            {
                TotalCount = totalCount,
                PageNo = pageNo,
                PageSize = pageSize,
                Data = students
            });
        }
    }
}
