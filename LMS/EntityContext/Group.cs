using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LMS.EntityСontext
{
    public class Group
    {
        public Group()
        {
            Users = new List<User>();
        }

        public int GroupId { get; set; }

        [Required]
        [StringLength(256, MinimumLength = 5)]
        public string Name { get; set; }

        [Required]
        [Range(2019, 2100)]
        public int Year { get; set; }

        public ICollection<User> Users { get; set; }
    }
}