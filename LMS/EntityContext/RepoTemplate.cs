using System.ComponentModel.DataAnnotations;

namespace LMS.EntityСontext
{
    public class RepoTemplate
    {
        public int RepoTemplateId { get; set; }

        [Required]
        public string Description { get; set; }
    }
}