using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.EntityСontext
{
    public class Course
    {
        public Course()
        {
            Users = new List<User>();
            LaboratoryWorks = new List<LaboratoryWork>();
        }

        public int CourseId { get; set; }

        public int UserId { get; set; }//Создатель

        public User User { get; set; }//Создатель

        [Required]
        [StringLength(256, MinimumLength = 5)]
        public string Name { get; set; }

        public ICollection<User> Users { get; set; }//Участники

        public ICollection<LaboratoryWork> LaboratoryWorks { get; set; }
    }
}