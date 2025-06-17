using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Models;

namespace DataAccessLayer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<History> Histories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<DifficultyLevel> DifficultyLevels { get; set; }
        public DbSet<QuestionType> QuestionTypes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<SampleEntity> SampleEntities { get; set; }
        public DbSet<Lecture> lectures { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<ExamSettings> ExamSettings { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<User>()
                .HasOne(u => u.Roles)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            modelBuilder.Entity<History>()
                .HasOne(h => h.User)
                .WithMany(u => u.Histories)
                .HasForeignKey(h => h.UserId);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Course)
                .WithMany(c => c.Questions)
                .HasForeignKey(q => q.CourseId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.DifficultyLevel)
                .WithMany(dl => dl.Questions)
                .HasForeignKey(q => q.DifficultyLevelId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Professor)
                .WithMany(u => u.Questions)
                .HasForeignKey(q => q.ProfessorId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Question>()
                .HasOne(q => q.QuestionType)
                .WithMany(qt => qt.Questions)
                .HasForeignKey(q => q.QuestionTypeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Category)
                .WithMany(c => c.Questions)
                .HasForeignKey(q => q.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Category)
                .WithMany(c => c.Courses)
                .HasForeignKey(c => c.CategoryId);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Professor)
                .WithMany(p => p.Courses)
                .HasForeignKey(c => c.ProfessorId);
           
           
            modelBuilder.Entity<Category>()
                .HasOne(c => c.Professor)
                .WithMany(p => p.Categories)
                .HasForeignKey(c => c.ProfessorId);
          

            modelBuilder.Entity<Question>()
                .Property(q => q.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

            //modelBuilder.Entity<Answer>()
            //    .HasOne(a => a.Question)
            //    .WithMany(q => q.Answers)
            //    .HasForeignKey(a => a.QuestionId);
            modelBuilder.Entity<Answer>()
               .HasOne(a => a.Question)
               .WithMany(q => q.Answers)
               .HasForeignKey(a => a.QuestionId)
               .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Lecture>()
                .HasOne(l => l.Course)
                .WithMany(c => c.Lectures)
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Lecture)
                .WithMany(l => l.Questions)
                .HasForeignKey(q => q.LectureId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Lecture>()
                .HasOne(l => l.Professor) // A lecture has one professor
                .WithMany(u => u.Lectures) // A professor has many lectures
                .HasForeignKey(l => l.ProfessorId) // Foreign key in Lecture
                .OnDelete(DeleteBehavior.NoAction); // Prevent cascading delete
                                                    // Exam → Course (Many-to-One)
            modelBuilder.Entity<Exam>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Exams)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.NoAction);

            // Exam → Professor (Many-to-One)
            modelBuilder.Entity<Exam>()
                .HasOne(e => e.Professor)
                .WithMany(p => p.Exams)
                .HasForeignKey(e => e.ProfessorId)
                .OnDelete(DeleteBehavior.NoAction);

            // Exam → ExamSettings (One-to-One)
            modelBuilder.Entity<Exam>()
                .HasOne(e => e.ExamSettings)
                .WithOne(es => es.Exam)
                .HasForeignKey<ExamSettings>(es => es.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            // ExamQuestion → Exam (Many-to-One)
            modelBuilder.Entity<ExamQuestion>()
                .HasOne(eq => eq.Exam)
                .WithMany(e => e.ExamQuestions)
                .HasForeignKey(eq => eq.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            // ExamQuestion → Question (Many-to-One)
            modelBuilder.Entity<ExamQuestion>()
                .HasOne(eq => eq.Question)
                .WithMany()
                .HasForeignKey(eq => eq.QuestionId)
                .OnDelete(DeleteBehavior.NoAction);


            // Seed data
            SeedInitialRoles(modelBuilder);
            SeedAdminUser(modelBuilder);
            SeedQuestionTypes(modelBuilder);
            SeedDifficultyLevels(modelBuilder);
            //SeedCategories(modelBuilder);
            SeedInitialCourses(modelBuilder);            
            //SeedAnswers(modelBuilder);
        }

        private void SeedInitialRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Professor" },
                new Role { Id = 2, Name = "Student" },
                new Role { Id = 3, Name = "Admin" }
            );
        }
        private void SeedInitialCourses(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>().HasData(
                new Course { Id = 3, Name = "data1", CategoryId = 2, ProfessorId = 2 }

            );
        }
        private void SeedAdminUser(ModelBuilder modelBuilder)
        {
            var passwordHasher = new PasswordHasher<User>();
            var adminUser = new User
            {
                Id = 1,
                Email = "Admin@admin.com",
                PasswordHash = passwordHasher.HashPassword(null, "Admin12335"),
                RoleId = 3
            };
            modelBuilder.Entity<User>().HasData(adminUser);

         
        }
        private void SeedQuestionTypes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuestionType>().HasData(
                new QuestionType { Id = 1, Type = "Multiple Choice" },
                new QuestionType { Id = 2, Type = "True/False" }
            );
        }
        private void SeedDifficultyLevels(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DifficultyLevel>().HasData(
                new DifficultyLevel { Id = 1, Level = "Easy" },
                new DifficultyLevel { Id = 2, Level = "Medium" },
                new DifficultyLevel { Id = 3, Level = "Hard" }
            );
        }
        
    }
}
