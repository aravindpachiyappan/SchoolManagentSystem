namespace SchoolManagentSystem_API.Dtos.SchoolDTO
{
    public class CreateSchoolRequestDTO
    {
        public string? SchoolName { get; set; }
        public string? SchoolType { get; set; }
        public string? SchoolLocation { get; set; }
        public string? SchoolCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }
}
