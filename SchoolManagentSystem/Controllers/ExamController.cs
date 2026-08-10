using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagentSystem_API.DBContext;
using SchoolManagentSystem_API.Dtos.ExamDTO;

namespace SchoolManagentSystem_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExamController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("exam-list")]
        public async Task<ActionResult<List<ExamListsResponseDTO>>> ExamList(ExamListRequestDTO request)
        {
            var query = from exam in _context.Exams
                        join cls in _context.Classes on exam.ClassID equals cls.ClassID into clsGroup
                        from cls in clsGroup.DefaultIfEmpty()
                        join school in _context.Schools on (cls != null ? cls.SchoolID : 0) equals school.SchoolID into schoolGroup
                        from school in schoolGroup.DefaultIfEmpty()
                        where exam.IsActive && !exam.IsDeleted
                        select new
                        {
                            exam,
                            clsName = cls != null ? cls.ClassName : string.Empty,
                            schoolId = cls != null ? cls.SchoolID : 0,
                            schoolName = school != null ? school.SchoolName : string.Empty
                        };

            // Search
            if (!string.IsNullOrWhiteSpace(request.SearchString))
            {
                var search = request.SearchString.Trim().ToLower();

                query = query.Where(x =>
                    x.exam.ExamName.ToLower().Contains(search) ||
                    x.exam.ExamType.ToLower().Contains(search) ||
                    x.clsName.ToLower().Contains(search) ||
                    x.schoolName.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync();

            // Sanitize & Auto-correct page number if out of bounds
            var pageNo = request.PageNo <= 0 ? 1 : request.PageNo;
            var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

            if ((pageNo - 1) * pageSize >= totalCount && totalCount > 0)
            {
                pageNo = 1;
            }

            var exams = await query
                .OrderBy(x => x.exam.ExamID)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ExamListsResponseDTO
                {
                    ExamID = x.exam.ExamID,
                    ExamName = x.exam.ExamName,
                    ExamType = x.exam.ExamType,
                    ExamDate = x.exam.ExamDate,
                    TotalMarks = x.exam.TotalMarks,
                    ClassID = x.exam.ClassID,
                    ClassName = x.clsName,
                    SchoolID = x.schoolId,
                    SchoolName = x.schoolName
                })
                .ToListAsync();

            return Ok(new
            {
                TotalCount = totalCount,
                PageNo = pageNo,
                PageSize = pageSize,
                Data = exams
            });
        }
    }
}
