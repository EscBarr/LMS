using System.ComponentModel.DataAnnotations;
using LMS.EntityСontext;

namespace LMS.DTO
{
    public class UserDTO
    {
        [Required]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string Patronymic { get; set; }
    }
}