namespace SchoolManagentSystem_API.Dtos.StudentDTO
{
    public class GetAllStudentListDetailsResponseDTO
    {
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? RollNumber { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public DateTime StudentBirthDate { get; set; }
        public string? BloodGroup { get; set; }
        public string? Address { get; set; }
        public string? FatherName { get; set; }
        public string? MotherName { get; set; }
        public string? FatherPhoneNumber { get; set; }
        public string? MotherPhoneNumber { get; set; }
        public string? ClassName { get; set; }
        public string? SchoolName { get; set; }
        public int? SchoolID { get; set; }
        public int ClassID { get; set; }
    }
}
