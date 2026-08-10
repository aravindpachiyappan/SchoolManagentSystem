namespace SchoolManagentSystem_API.Dtos.ExamDTO
{
    public class ExamListsResponseDTO
    {
            public int ExamID { get; set; }

            public string ExamName { get; set; } = string.Empty;

            public string ExamType { get; set; } = string.Empty;

            public DateTime ExamDate { get; set; }

            public int ClassID { get; set; }

            public string ClassName { get; set; } = string.Empty;

            public int SchoolID { get; set; }

            public string SchoolName { get; set; } = string.Empty;

            public int TotalMarks { get; set; }
    }
}
