namespace SchoolManagentSystem_API.Dtos.ClassDTO
{
    public class GetAllDetailsResponseDTO
    {
        public int ClassID { get; set; }

        public string ClassName { get; set; } = string.Empty;

        public string ClassLocation { get; set; } = string.Empty;

        public string Section { get; set; } = string.Empty;

        public int Strength { get; set; }

        public int SchoolID { get; set; }
    }
}
