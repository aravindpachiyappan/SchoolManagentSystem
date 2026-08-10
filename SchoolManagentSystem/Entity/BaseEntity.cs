namespace SchoolManagentSystem_API.Entity
{
    public class BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
