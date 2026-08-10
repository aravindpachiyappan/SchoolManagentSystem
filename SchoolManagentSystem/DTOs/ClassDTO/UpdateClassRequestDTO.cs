namespace SchoolManagentSystem_API.Dtos.ClassDTO
{
    public class UpdateClassRequestDTO
    {
        public int ClassID { get; set; }
        public string? ClassName { get; set; }
        public string? ClassLocation { get; set; }
        public string? Section { get; set; } // A, B, C
        public int Strength { get; set; }    // Student Count
        public int SchoolID { get; set; }
    }
}
