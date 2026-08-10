using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagentSystem_API.Entity
{
    public class Marks : BaseEntity
    {
        [Key]
        public int MarkID { get; set; }
        public int ClassID { get; set; }
        public int StudentID { get; set; }
        public int ExamID { get; set; }
        public int SubjectID { get; set; }
        public decimal ObtainedMarks { get; set; }
        public decimal TotalMarks { get; set; }

        [ForeignKey(nameof(StudentID))]
        public Students? Student { get; set; }

        [ForeignKey(nameof(ExamID))]
        public Exam? Exam { get; set; }

        [ForeignKey(nameof(SubjectID))]
        public Subjects? Subject { get; set; }
    }
}
