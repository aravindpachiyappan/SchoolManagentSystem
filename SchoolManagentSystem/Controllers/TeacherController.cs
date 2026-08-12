using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagentSystem_API.DBContext;
using SchoolManagentSystem_API.Dtos.TeacherDTO;
using SchoolManagentSystem_API.Entity;

namespace SchoolManagentSystem_API.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TeacherController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTeacher(CreateTeacherRequestDTO CreateTeacherRequest)
        {
            var CreateQueryTeacher = await _context.Teachers.FirstOrDefaultAsync(x => (x.PhoneNumber == CreateTeacherRequest.PhoneNumber
                                                                                  || x.Email == CreateTeacherRequest.Email)
                                                                                  && !x.IsDeleted);
            if (CreateQueryTeacher != null)
            {
                return BadRequest("Email or Phone number already exists.");
            }

            var CreateTeachers = new Teacher
            {
                TeacherName = CreateTeacherRequest.TeacherName,
                Gender = CreateTeacherRequest.Gender,
                Qualification = CreateTeacherRequest.Qualification,
                Address = CreateTeacherRequest.Address,
                PhoneNumber = CreateTeacherRequest.PhoneNumber,
                Email = CreateTeacherRequest.Email,
                Salary = CreateTeacherRequest.Salary,
                JoiningDate = CreateTeacherRequest.JoiningDate,
                SchoolID = CreateTeacherRequest.SchoolID,
                ClassID = CreateTeacherRequest.ClassID,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                IsActive = true,
                IsDeleted = false
            };

            _context.Teachers.Add(CreateTeachers);
            await _context.SaveChangesAsync();
            return Ok("Teacher created successfully.");
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateTeacherDetails(UpdateTeacherRequestDTO UpdateTeacherRequest)
        {
            var UpdateQuery = await _context.Teachers.FirstOrDefaultAsync(x => x.TeacherID == UpdateTeacherRequest.TeacherID && x.IsActive && !x.IsDeleted);

            if (UpdateQuery == null)
            {
                return NotFound("Enter Valid Teacher ID");
            }

            UpdateQuery.TeacherName = UpdateTeacherRequest.TeacherName;
            UpdateQuery.Gender = UpdateTeacherRequest.Gender;
            UpdateQuery.Qualification = UpdateTeacherRequest.Qualification;
            UpdateQuery.Address = UpdateTeacherRequest.Address;
            UpdateQuery.PhoneNumber = UpdateTeacherRequest.PhoneNumber;
            UpdateQuery.Email = UpdateTeacherRequest.Email;
            UpdateQuery.Salary = UpdateTeacherRequest.Salary;
            UpdateQuery.JoiningDate = UpdateTeacherRequest.JoiningDate;
            UpdateQuery.SchoolID = UpdateTeacherRequest.SchoolID;
            UpdateQuery.ClassID = UpdateTeacherRequest.ClassID;
            UpdateQuery.UpdatedAt = DateTime.UtcNow;
            UpdateQuery.UpdatedBy = 1;

            _context.Teachers.Update(UpdateQuery);
            await _context.SaveChangesAsync();

            return Ok("Teacher Records Updated Successfully");
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteTeacherRecords(TeacherDetailsDeleteDTO DeleteRequest)
        {
            var deleteQuery = await _context.Teachers.FindAsync(DeleteRequest.TeacherID);

            if (deleteQuery == null)
            {
                throw new Exception("Teacher Not found");
            }

            deleteQuery.IsActive = false;
            deleteQuery.IsDeleted = true;
            deleteQuery.UpdatedAt = DateTime.UtcNow;
            deleteQuery.UpdatedBy = 1;

            _context.Teachers.Update(deleteQuery);
            await _context.SaveChangesAsync();

            return Ok("Records Deleted Successfully");
        }

        [HttpPost("getbyid")]
        public async Task<IActionResult> GetOneteacherRecords(GetOneTeacherRequestDTO TeacherRecordsDTO)
        {
            var getAllQuery = await _context.Teachers.FindAsync(TeacherRecordsDTO.TeacherID);

            if (getAllQuery == null)
            {
                throw new Exception("Teacher Id Not Found");
            }

            var response = new GetOneTeacherRecordsResponseDTO
            {
                TeacherName = getAllQuery.TeacherName,
                Gender = getAllQuery.Gender,
                Qualification = getAllQuery.Qualification,
                Address = getAllQuery.Address,
                PhoneNumber = getAllQuery.PhoneNumber,
                Email = getAllQuery.Email,
                Salary = getAllQuery.Salary,
                JoiningDate = getAllQuery.JoiningDate,
                SchoolID = getAllQuery.SchoolID,
                SchoolName = getAllQuery.School?.SchoolName,
                ClassID = getAllQuery.ClassID,
                ClassName = getAllQuery.Class?.ClassName
            };

            return Ok(response);
        }

        [HttpPost("getall")]
        public async Task<ActionResult> GetAllTeacherRecords(
     GetAllTeacherRecordRequestDTO request)
        {
            var query =
                from teacher in _context.Teachers
                join school in _context.Schools
                    on teacher.SchoolID equals school.SchoolID
                join cls in _context.Classes
                    on teacher.ClassID equals cls.ClassID
                where teacher.IsActive && !teacher.IsDeleted
                select new
                {
                    Teacher = teacher,
                    SchoolName = school.SchoolName,
                    ClassName = cls.ClassName
                };

            // Search
            if (!string.IsNullOrWhiteSpace(request.SearchString))
            {
                var search = request.SearchString.Trim().ToLower();

                query = query.Where(x =>
                    x.Teacher.TeacherName.ToLower().Contains(search) ||
                    x.Teacher.Email.ToLower().Contains(search) ||
                    x.Teacher.PhoneNumber.Contains(search) ||
                    x.Teacher.Qualification.ToLower().Contains(search) ||
                    x.SchoolName.ToLower().Contains(search) ||
                    x.ClassName.ToLower().Contains(search));
            }

            // Total count
            var totalCount = await query.CountAsync();

            // Pagination
            var pageNo = request.PageNo <= 0 ? 1 : request.PageNo;
            var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

            if ((pageNo - 1) * pageSize >= totalCount && totalCount > 0)
            {
                pageNo = 1;
            }

            var teachers = await query
                .OrderBy(x => x.Teacher.TeacherID)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new GetAllTeacherRecordsResponseDTO
                {
                    TeacherID = x.Teacher.TeacherID,
                    TeacherName = x.Teacher.TeacherName,
                    Gender = x.Teacher.Gender,
                    Qualification = x.Teacher.Qualification,
                    Address = x.Teacher.Address,
                    PhoneNumber = x.Teacher.PhoneNumber,
                    Email = x.Teacher.Email,
                    Salary = x.Teacher.Salary,
                    JoiningDate = x.Teacher.JoiningDate,

                    SchoolID = x.Teacher.SchoolID,
                    SchoolName = x.SchoolName,

                    ClassID = x.Teacher.ClassID,
                    ClassName = x.ClassName
                })
                .ToListAsync();

            return Ok(new
            {
                TotalCount = totalCount,
                PageNo = pageNo,
                PageSize = pageSize,
                Data = teachers
            });
        }
    }
}
