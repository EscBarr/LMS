using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.EntityСontext
{
    public class User
    {
        public User()
        {
            LaboratoryWorks = new List<LaboratoryWork>();
            AssignedVariants = new List<AssignedVariant>();
            UserRoles = new List<UserRole>();
            Courses = new List<Course>();
            CreatedCourses = new List<Course>();
            Repositories = new List<RepositoryEntity>();
        }

        public int Id { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string PwHash { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public int? GroupId { get; set; }

        public string? GitUsername { get; set; }

        public ICollection<LaboratoryWork> LaboratoryWorks { get; set; } //Созданные задания?
        public ICollection<AssignedVariant> AssignedVariants { get; set; } //Полученные задания
        public ICollection<UserRole> UserRoles { get; set; } //Роли

        public ICollection<Course> Courses { get; set; } = new List<Course>(); //Список курсов пользователя (в которых он является участником)

        public ICollection<Course> CreatedCourses { get; set; } //Список курсов созданных пользователем

        public ICollection<RepositoryEntity> Repositories { get; set; } //Список принадлежащих репозиториев
        public Group Group { get; set; }
    }
}