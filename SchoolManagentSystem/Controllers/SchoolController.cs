using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagentSystem_API.DBContext;
using SchoolManagentSystem_API.Dtos.SchoolDTO;
using SchoolManagentSystem_API.Entity;

namespace SchoolManagentSystem_API.Controllers
{
    [Authorize(Roles = "Admin,Teacher,Student")]
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SchoolController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost()]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSchools(CreateSchoolRequestDTO request)
        {
            //Validation
            //Check Already The SchoolName, SchoolCode In DB
            var existingSchool = await _context.Schools.FirstOrDefaultAsync(x => (x.SchoolName == request.SchoolName
                                                                           || x.SchoolCode == request.SchoolCode)
                                                                           && !x.IsDeleted);

            if (existingSchool != null)
            {
                return BadRequest("School with this name or code already exists.");
            }

            //Create Data
            var CreateSchoolModel = new School
            {
                SchoolName = request.SchoolName,
                SchoolCode = request.SchoolCode,
                SchoolLocation = request.SchoolLocation,
                SchoolType = request.SchoolType,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                IsActive = true,
                IsDeleted = false,

            };
            _context.Schools.Add(CreateSchoolModel);

            //Save Data In DB
            await _context.SaveChangesAsync();

            //Response
            return Ok("School Created Successfully");
        }

        [HttpPost("id")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSchool(UpdateSchoolRequestDTO request)
        {
            var query = await _context.Schools.FirstOrDefaultAsync(x => x.SchoolID == request.SchoolID && x.IsActive && !x.IsDeleted);

            if (query == null)
            {
                return NotFound("School Id not found.");
            }

            query.SchoolName = request.SchoolName;
            query.SchoolCode = request.SchoolCode;
            query.SchoolLocation = request.SchoolLocation;
            query.SchoolType = request.SchoolType;
            query.UpdatedAt = DateTime.UtcNow;
            query.UpdatedBy = 1;

            _context.Schools.Update(query);

            //Save Data In DB
            await _context.SaveChangesAsync();

            //Response
            return Ok("Updated Successfully");
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(DeleteSchoolRequestDTO request)
        {
            var query = await _context.Schools.FindAsync(request.SchoolID);

            if (query == null)
            {
                return NotFound("School Id Not Found");
            }

            query.IsActive = false;
            query.IsDeleted = true;
            query.UpdatedAt = DateTime.UtcNow;
            query.UpdatedBy = 1;

            _context.Schools.Update(query);
            await _context.SaveChangesAsync();
            return Ok("Deleted Successfully");
        }

        [HttpPost("getall")]
        public async Task<ActionResult<List<SchoolListResponseDTO>>> GetAllSchoolDetails(GetAllSchoolRequestDTO request)
        {
            var query = _context.Schools
                .Where(x => x.IsActive && !x.IsDeleted)
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(request.SearchString))
            {
                var search = request.SearchString.Trim().ToLower();

                query = query.Where(x =>
                    x.SchoolName.ToLower().Contains(search) ||
                    x.SchoolCode.ToLower().Contains(search) ||
                    x.Email.ToLower().Contains(search) ||
                    x.PhoneNumber.Contains(search));
            }

            // Total Count (before pagination)
            var totalCount = await query.CountAsync();

            // Sanitize & Auto-correct page number if out of bounds
            var pageNo = request.PageNo <= 0 ? 1 : request.PageNo;
            var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

            if ((pageNo - 1) * pageSize >= totalCount && totalCount > 0)
            {
                pageNo = 1;
            }

            // Pagination
            var schools = await query
                .OrderBy(x => x.SchoolID)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new SchoolListResponseDTO
                {
                    SchoolID = x.SchoolID,
                    SchoolName = x.SchoolName,
                    SchoolCode = x.SchoolCode,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber
                })
                .ToListAsync();

            return Ok(new
            {
                TotalCount = totalCount,
                PageNo = pageNo,
                PageSize = pageSize,
                Data = schools
            });
        }

        //[HttpPost("geybyid")]
        //public async Task<IActionResult> GetById(SchoolGetByIDRequestDTO request)
        //{
        //    var query = await _context.Schools.FindAsync(request.SchoolID);

        //    if (query == null)
        //    {
        //        throw new Exception("School Id Not Founded");
        //    }

        //    query.IsActive = true;
        //    query.IsDeleted = false;
        //    query.UpdatedAt = DateTime.UtcNow;
        //    query.UpdatedBy = 1;

        //    _context.Schools.Update(query);
        //    await _context.SaveChangesAsync();
        //    return Ok("Deleted Successfully");
        //}

        [HttpPost("getbyid")]
        public async Task<IActionResult> GetById(SchoolGetByIDRequestDTO request)
        {
            var school = await _context.Schools.FindAsync(request.SchoolID);

            if (school == null)
            {
                return NotFound("School ID not found.");
            }

            var response = new SchoolGetByIDResponseDTO
            {
                SchoolID = school.SchoolID,
                SchoolName = school.SchoolName,
                SchoolLocation = school.SchoolLocation,
                SchoolCode = school.SchoolCode,
                SchoolType = school.SchoolType,
                Email = school.Email,
                PhoneNumber = school.PhoneNumber,
            };

            return Ok(response);
        }

        [HttpPost("search")]
        public async Task<IActionResult> search(SearchSchoolRequestDTO request)
        {
            var searchQuery = await _context.Schools.Where(x => x.IsActive == true
                                                           && x.IsDeleted == false)
                                                           .ToListAsync();

            if (!string.IsNullOrEmpty(request.SearchString))
            {
                searchQuery = searchQuery.Where(x => x.PhoneNumber.Contains(request.SearchString)).ToList();
            }

            var searchResult = searchQuery.OrderBy(x => x.SchoolID)
                                          .Select(x => new SchoolListResponseDTO
                                          {
                                              SchoolName = x.SchoolName,
                                              SchoolCode = x.SchoolCode,
                                              Email = x.Email,
                                              PhoneNumber = x.PhoneNumber
                                          }).ToList();
            return Ok(searchResult);
        }
    }
}
