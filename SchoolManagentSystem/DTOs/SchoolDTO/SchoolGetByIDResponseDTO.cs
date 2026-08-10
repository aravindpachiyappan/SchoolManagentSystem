namespace SchoolManagentSystem_API.Dtos.SchoolDTO
{
    public class SchoolGetByIDResponseDTO
    {
        public int SchoolID { get; set; }
        public string? SchoolName { get; set; }
        public string? SchoolType { get; set; }
        public string? SchoolLocation { get; set; }
        public string? SchoolCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }
}
