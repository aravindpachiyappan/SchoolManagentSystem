namespace SchoolManagentSystem_API.Dtos.TeacherDTO
{
    public class UpdateTeacherRequestDTO
    {
        public int TeacherID { get; set; }
        public string? TeacherName { get; set; }
        public string? Gender { get; set; }
        public string? Qualification { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public decimal Salary { get; set; }
        public DateTime JoiningDate { get; set; }
        public int SchoolID { get; set; }
        public int ClassID { get; set; }
    }
}
