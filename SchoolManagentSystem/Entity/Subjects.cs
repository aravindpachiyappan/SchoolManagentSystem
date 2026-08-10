using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagentSystem_API.Entity
{
    public class Subjects : BaseEntity
    {
        [Key]
        public int SubjectID { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
    }
}
