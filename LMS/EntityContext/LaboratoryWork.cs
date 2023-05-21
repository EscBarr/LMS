using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.EntityСontext
{
    public class LaboratoryWork
    {
        public LaboratoryWork()
        {
            Variants = new List<Variant>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 5)]
        public string Name { get; set; }

        [Required]
        [StringLength(256, MinimumLength = 5)]
        public string Description { get; set; }

        public int MaxMark { get; set; }

        public int UserId { get; set; }//Создатель

        public int CourseId { get; set; }//Курс

        //public DateTime DueDateTime { get; set; }//Крайний срок выполнения

        public User User { get; set; }

        public Course Course { get; set; }

        public ICollection<Variant> Variants { get; set; }
    }
}