using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagentSystem_API.Entity
{
    public class UserRoles : BaseEntity
    {
        [Key]
        public int? UserRolesID { get; set; }

        public int? RoleID { get; set; }

        public int? UserID { get; set; }

        [ForeignKey(nameof(RoleID))]
        public Roles? Roles { get; set; }

        [ForeignKey(nameof(UserID))]
        public User? User { get; set; }
    }
}
