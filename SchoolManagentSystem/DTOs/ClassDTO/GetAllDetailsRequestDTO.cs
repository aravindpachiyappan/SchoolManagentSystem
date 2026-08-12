namespace SchoolManagentSystem_API.Dtos.ClassDTO
{
    public class GetAllDetailsRequestDTO
    {
        public string? SearchString { get; set; }

        public int? SchoolId { get; set; }
        public int PageNo { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
