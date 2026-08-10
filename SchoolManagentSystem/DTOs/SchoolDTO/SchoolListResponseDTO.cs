namespace SchoolManagentSystem_API.Dtos.SchoolDTO
{
    public class SchoolListResponseDTO
    {
        public int SchoolID { get; set; }
        public string? SchoolName { get; set; }
        public string? SchoolCode { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
