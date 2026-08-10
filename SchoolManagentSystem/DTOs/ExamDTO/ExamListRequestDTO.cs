namespace SchoolManagentSystem_API.Dtos.ExamDTO
{
    public class ExamListRequestDTO
    {
        public string SearchString { get; set; } = string.Empty;
        public int PageNo { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
