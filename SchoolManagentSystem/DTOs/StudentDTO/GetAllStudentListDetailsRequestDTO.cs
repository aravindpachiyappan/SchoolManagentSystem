namespace SchoolManagentSystem_API.Dtos.StudentDTO
{
    public class GetAllStudentListDetailsRequestDTO
    {
        public string SearchString { get; set; } = string.Empty;
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
