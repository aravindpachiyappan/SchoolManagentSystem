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
        public async Task<ActionResult<List<GetAllStudentListDetailsResponseDTO>>> GetAllStudentDetails(GetAllStudentListDetailsRequestDTO request)
        {
            var query = _context.Students
    .Where(x => x.IsActive && !x.IsDeleted)
    .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(request.SearchString))
            {
                var search = request.SearchString.Trim().ToLower();

                query = query.Where(x =>
                    x.StudentName.ToLower().Contains(search) ||
                    x.RollNumber.ToLower().Contains(search) ||
                    x.Email.ToLower().Contains(search) ||
                    x.FatherName.ToLower().Contains(search) ||
                    x.MotherName.ToLower().Contains(search) ||
                    x.FatherPhoneNumber.Contains(search) ||
                    x.MotherPhoneNumber.Contains(search));
            }

            var totalCount = await query.CountAsync();

            // Sanitize & Auto-correct page number if out of bounds
            var pageNo = request.PageNo <= 0 ? 1 : request.PageNo;
            var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

            if ((pageNo - 1) * pageSize >= totalCount && totalCount > 0)
            {
                pageNo = 1;
            }

            var students = await query
                .OrderBy(x => x.StudentId)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new GetAllStudentListDetailsResponseDTO
                {
                    StudentId = x.StudentId,
                    StudentName = x.StudentName,
                    RollNumber = x.RollNumber,
                    Email = x.Email,
                    Gender = x.Gender,
                    StudentBirthDate = x.StudentBirthDate,
                    BloodGroup = x.BloodGroup,
                    Address = x.Address,
                    FatherName = x.FatherName,
                    MotherName = x.MotherName,
                    FatherPhoneNumber = x.FatherPhoneNumber,
                    MotherPhoneNumber = x.MotherPhoneNumber,
                    SchoolID = x.SchoolID,
                    ClassID = x.ClassID,
                    ClassName = x.Class.ClassName,
                    SchoolName = x.School.SchoolName
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
