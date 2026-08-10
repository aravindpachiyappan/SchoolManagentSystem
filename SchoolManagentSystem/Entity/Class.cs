using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagentSystem_API.Entity
{
    public class Class : BaseEntity
    {
        [Key]
        public int ClassID { get; set; }
        public string? ClassName { get; set; }
        public string? ClassLocation { get; set; }
        public string? Section { get; set; } // A, B, C
        public int Strength { get; set; }    // Student Count
        public int SchoolID { get; set; }


        [ForeignKey(nameof(SchoolID))]
        public School? School { get; set; }
    }
}
