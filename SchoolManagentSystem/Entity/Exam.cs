using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagentSystem_API.Entity
{
    public class Exam : BaseEntity
    {
        [Key]
        public int ExamID { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public string ExamType { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
        public int ClassID { get; set; }
        public int TotalMarks { get; set; }

        [ForeignKey(nameof(ClassID))]
        public Class? Class { get; set; }
    }
}
