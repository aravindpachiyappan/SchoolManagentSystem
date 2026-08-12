using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagentSystem_API.DBContext;
using SchoolManagentSystem_API.Dtos.ClassDTO;
using SchoolManagentSystem_API.Entity;

namespace SchoolManagentSystem_API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ClassController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateClass(CreateClassDTO createClassRequest)
        {
            var createQuery = await _context.Classes.FirstOrDefaultAsync(x => x.ClassName == createClassRequest.ClassName
                                                                         && x.Section == createClassRequest.Section
                                                                         && x.SchoolID == createClassRequest.SchoolID
                                                                         && !x.IsDeleted);
            if (createQuery != null)
            {
                return BadRequest("Class with this name and section already exists in the school.");
            }

            var createNewClass = new Class
            {
                ClassName = createClassRequest.ClassName,
                ClassLocation = createClassRequest.ClassLocation,
                Section = createClassRequest.Section,
                Strength = createClassRequest.Strength,
                SchoolID = createClassRequest.SchoolID,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                IsActive = true,
                IsDeleted = false,
            };
            _context.Classes.Add(createNewClass);

            //Save Data In DB.
            await _context.SaveChangesAsync();

            //Return Response.
            return Ok("Create Successfully");
        }

        [HttpPost("update")]
        public async Task<IActionResult> ClassUpdate(UpdateClassRequestDTO updateRequest)
        {
            var query = await _context.Classes.FirstOrDefaultAsync(x => x.ClassID == updateRequest.ClassID && x.IsActive && !x.IsDeleted);

            if (query == null)
            {
                return NotFound("Class ID not found.");
            }

            query.ClassName = updateRequest.ClassName;
            query.ClassLocation = updateRequest.ClassLocation;
            query.Section = updateRequest.Section;
            query.Strength = updateRequest.Strength;
            query.UpdatedAt = DateTime.UtcNow;
            query.UpdatedBy = 1;

            _context.Classes.Update(query);

            await _context.SaveChangesAsync();

            return Ok("Class Updated Successfully");
        }

        [HttpPost("delete")]
        public async Task<IActionResult> classDelete(ClassDeleteRequestDTO deleteRequest)
        {
            var query = await _context.Classes.FindAsync(deleteRequest.ClassID);
            if (query == null)
            {
                return NotFound("Class ID not found.");
            }

            query.IsActive = false;
            query.IsDeleted = true;
            query.UpdatedAt = DateTime.UtcNow;
            query.UpdatedBy = 1;

            _context.Classes.Update(query);

            await _context.SaveChangesAsync();

            return Ok("Deleted Successfully");
        }

        [HttpPost("getall")]
        public async Task<ActionResult<List<GetAllDetailsResponseDTO>>> GetAllDetails(GetAllDetailsRequestDTO request)
        {
            var query = _context.Classes
                .Where(x => x.IsActive && !x.IsDeleted)
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(request.SearchString))
            {
                var search = request.SearchString.Trim().ToLower();

                query = query.Where(x =>
                    x.ClassName.ToLower().Contains(search) ||
                    x.ClassLocation.ToLower().Contains(search) ||
                    x.Section.ToLower().Contains(search));
            }

            // Filter by SchoolId
            if (request.SchoolId.HasValue)
            {
                query = query.Where(x => x.SchoolID == request.SchoolId.Value);
            }

            // Total Count
            var totalCount = await query.CountAsync();

            // Sanitize & Auto-correct page number if out of bounds
            var pageNo = request.PageNo <= 0 ? 1 : request.PageNo;
            var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

            if ((pageNo - 1) * pageSize >= totalCount && totalCount > 0)
            {
                pageNo = 1;
            }

            // Pagination
            var classes = await query
                .OrderBy(x => x.ClassID)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new GetAllDetailsResponseDTO
                {
                    ClassID = x.ClassID,
                    ClassName = x.ClassName,
                    ClassLocation = x.ClassLocation,
                    Section = x.Section,
                    Strength = x.Strength,
                    SchoolID = x.SchoolID
                })
                .ToListAsync();

            return Ok(new
            {
                TotalCount = totalCount,
                PageNo = pageNo,
                PageSize = pageSize,
                Data = classes
            });
        }
    }
}
