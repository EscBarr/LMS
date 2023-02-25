using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LMS.EntityСontext
{
    public class Role
    {
        public Role()
        {
            UserRoles = new List<UserRole>();
        }

        public int RoleId { get; set; }

        [Required]
        public RoleEnum RoleName { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
    }
}