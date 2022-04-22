using Microsoft.EntityFrameworkCore;
using School.Entities;

namespace School.Model
{
    public class SchoolDBContext : DbContext
    {
        public SchoolDBContext(DbContextOptions<SchoolDBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                // 1 to
                .HasOne<Grade>(s => s.Grade)
                // many Relation
                .WithMany(g => g.Students)
                .HasForeignKey(s => s.GradeId)
                //If the principal/parent entity is deleted, then the foreign key values of the dependents/children will no longer match the primary or alternate key of any principal/parent.
                //This is an invalid state, and will cause a referential constraint violation in most databases. That's why we use 'Cascade Delete Behavior' to also delete the dependent/child entities.
                .OnDelete(DeleteBehavior.Cascade);  

            modelBuilder.Entity<Student>()
                // 1 to
                .HasOne<Address>(a => a.Adress)
                // 1 Relation
                .WithOne(b => b.Student)
                .HasForeignKey<Address>(ad => ad.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Grade> StudentGrades { get; set; }
        public DbSet<Address> StudentAddresses { get; set; }
    }
}
