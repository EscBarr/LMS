using System.ComponentModel.DataAnnotations;
using LMS.EntityСontext;

namespace LMS.DTO
{
    public class UserEditDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        public string Patronymic { get; set; }
    }
}