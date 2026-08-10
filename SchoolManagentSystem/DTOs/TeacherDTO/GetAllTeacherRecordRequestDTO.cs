namespace SchoolManagentSystem_API.Dtos.TeacherDTO
{
    public class GetAllTeacherRecordRequestDTO
    {
        public string? SearchString { get; set; }

        public int PageNo { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
