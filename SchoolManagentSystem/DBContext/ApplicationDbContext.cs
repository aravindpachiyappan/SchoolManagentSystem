using Microsoft.EntityFrameworkCore;
using SchoolManagentSystem_API.Entity;

namespace SchoolManagentSystem_API.DBContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<User> User { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Marks> Markss { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Students> Students { get; set; }
        public DbSet<Subjects> Subjects { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
    }
}
