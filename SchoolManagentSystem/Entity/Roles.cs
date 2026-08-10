using System.ComponentModel.DataAnnotations;

namespace SchoolManagentSystem_API.Entity
{
    public class Roles : BaseEntity
    {
        [Key]
        public int? RolesID { get; set; }

        public string? RoleName { get; set; }
    }
}
