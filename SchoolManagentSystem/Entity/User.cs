using System.ComponentModel.DataAnnotations;

namespace SchoolManagentSystem_API.Entity 
{ 
    public class User : BaseEntity
    {
        [Key]
        public int? UserID { get; set; }

        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
