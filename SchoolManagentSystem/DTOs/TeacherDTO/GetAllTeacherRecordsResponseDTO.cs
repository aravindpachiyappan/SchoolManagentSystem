namespace SchoolManagentSystem_API.Dtos.TeacherDTO
{
    public class GetAllTeacherRecordsResponseDTO
    {
        public int TeacherID { get; set; }

        public string TeacherName { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public string Qualification { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public decimal Salary { get; set; }

        public DateTime JoiningDate { get; set; }

        public int SchoolID { get; set; }

        public int ClassID { get; set; }
    }
}
