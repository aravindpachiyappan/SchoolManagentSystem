using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagentSystem_API.Entity
{
    public class Teacher : BaseEntity
    {
        [Key]
        public int TeacherID { get; set; }
        public string? TeacherName { get; set; }
        public string? Gender { get; set; }
        public string? Qualification { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public decimal Salary { get; set; }
        public DateTime JoiningDate { get; set; }
        public int SchoolID { get; set; }
        public int ClassID { get; set; }



        [ForeignKey(nameof(SchoolID))]
        public School? School { get; set; }

        [ForeignKey(nameof(ClassID))]
        public Class? Class { get; set; }
    }
}
