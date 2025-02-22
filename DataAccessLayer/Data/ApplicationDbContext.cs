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

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
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
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.DifficultyLevel)
                .WithMany(dl => dl.Questions)
                .HasForeignKey(q => q.DifficultyLevelId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Professor)
                .WithMany(u => u.Questions)
                .HasForeignKey(q => q.ProfessorId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Question>()
                .HasOne(q => q.QuestionType)
                .WithMany(qt => qt.Questions)
                .HasForeignKey(q => q.QuestionTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Category)
                .WithMany(c => c.Questions)
                .HasForeignKey(q => q.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

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

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId);

            // Seed data
            SeedInitialRoles(modelBuilder);
            SeedAdminUser(modelBuilder);
            SeedQuestionTypes(modelBuilder);
            SeedDifficultyLevels(modelBuilder);
            //SeedCategories(modelBuilder);
            //SeedCourses(modelBuilder);
            //SeedQuestions(modelBuilder);
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
        //    var professorUser = new User
        //    {
        //        Id = 2, // Adding a professor user
        //        Email = "professor@university.com",
        //        PasswordHash = passwordHasher.HashPassword(null, "Prof12345"),
        //        RoleId = 1
        //    };

        //    modelBuilder.Entity<User>().HasData(adminUser, professorUser);
        //}



        //private void SeedCategories(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Category>().HasData(
        //        new Category { Id = 1, Name = "Mathematics", UserId = 2 },
        //        new Category { Id = 2, Name = "Science", UserId = 2 }
        //    );
        //}

        //private void SeedCourses(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Course>().HasData(
        //        new Course { Id = 1, Name = "Algebra 101", CategoryId = 1, UserId = 2 },
        //        new Course { Id = 2, Name = "Biology 101", CategoryId = 2, UserId = 2 },
        //        new Course { Id = 3, Name = "Algebra 102", CategoryId = 1, UserId = 2 },
        //        new Course { Id = 4, Name = "Biology 102", CategoryId = 2, UserId = 2 }
        //    );
        //}

        //private void SeedQuestions(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Question>().HasData(
        //        new Question
        //        {
        //            Id = 1,
        //            Text = "What is 2 + 2?",
        //            QuestionTypeId = 1,
        //            DifficultyLevelId = 1,
        //            UserId = 2,
        //            CreatedDate = DateTime.UtcNow,
        //            CategoryId = 1,
        //            CourseId = 1
        //        },
        //        new Question
        //        {
        //            Id = 2,
        //            Text = "Is the Earth round?",
        //            QuestionTypeId = 2,
        //            DifficultyLevelId = 1,
        //            UserId = 2,
        //            CreatedDate = DateTime.UtcNow,
        //            CategoryId = 2,
        //            CourseId = 2
        //        }
        //    );
        //}

        //private void SeedAnswers(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Answer>().HasData(
        //        new Answer { Id = 1, Text = "4", IsCorrect = true, QuestionId = 1 },
        //        new Answer { Id = 2, Text = "3", IsCorrect = false, QuestionId = 1 },
        //        new Answer { Id = 3, Text = "Yes", IsCorrect = true, QuestionId = 2 },
        //        new Answer { Id = 4, Text = "No", IsCorrect = false, QuestionId = 2 }
        //    );
        //}
    }
}
