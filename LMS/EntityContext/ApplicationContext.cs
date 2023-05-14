using LMS.EntityContext;
using LMS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LMS.EntityСontext
{
    public class ApplicationContext : DbContext
    {
        public DbSet<AssignedVariant> AssignedVariants { get; set; }

        //public DbSet<ChatMessages> ChatMessages { get; set; }
        public DbSet<Course> Courses { get; set; }

        public DbSet<Group> Groups { get; set; }
        public DbSet<LaboratoryWork> LaboratoryWorks { get; set; }
        public DbSet<RepositoryEntity> Repos { get; set; }
        public DbSet<RepoTemplate> ReposTemplates { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }

        //public DbSet<UserCourses> UserCourses { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Variant> Variants { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        static ApplicationContext()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresEnum<RoleEnum>();

            modelBuilder.Entity<AssignedVariant>()
               .HasIndex(av => new { av.UserId, av.VariantId })
               .IsUnique();

            modelBuilder.Entity<Group>()
                .HasIndex(g => new { g.Name, g.Year })
                .IsUnique();

            //modelBuilder.Entity<User>()
            //    .HasIndex(u => new { u.Email, u.GitUsername })
            //    .IsUnique();

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.GitUsername).IsUnique();
            });

            modelBuilder.Entity<Variant>()
                .HasIndex(var => new { var.VariantNumber, var.LaboratoryWorkId })
                .IsUnique();

            modelBuilder.Entity<Course>().HasMany(t => t.Users).WithMany(p => p.Courses);

            modelBuilder.Entity<Course>().HasOne(t => t.User).WithMany(d => d.CreatedCourses).HasForeignKey(t => t.UserId);
            //modelBuilder.Entity<LaboratoryWork>().HasMany(t => t.).WithOne(d => d.Course).HasForeignKey(t => t.LaboratoryWorkId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //try
            //{
            //    optionsBuilder.UseNpgsql("Host=db;Port=5432;Database=LMS;Username=postgres;Password=root");
            //}
            //catch
            //{
            //    optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=LMS;Username=postgres;Password=root");
            //}

            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=LMS;Username=postgres;Password=root");
            //optionsBuilder.UseNpgsql("Host=db;Port=5432;Database=LMS;Username=postgres;Password=root");
        }
    }
}