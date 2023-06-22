using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.EntityСontext
{
    public class Variant
    {
        public Variant()
        {
            AssignedVariants = new List<AssignedVariant>();
        }

        public int VariantId { get; set; }
        public int LaboratoryWorkId { get; set; }

        public int? AttachedRepoId { get; set; }

        public int VariantNumber { get; set; }

        [StringLength(1024, MinimumLength = 5)]
        public string Description { get; set; }

        //public RepositoryEntity? AttachedRepo { get; set; }

        public ICollection<AssignedVariant> AssignedVariants { get; set; }
        public LaboratoryWork LaboratoryWork { get; set; }
    }
}