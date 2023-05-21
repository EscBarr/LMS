using System;
using System.Collections.Generic;

namespace LMS.EntityСontext
{
    public class AssignedVariant
    {
        public int AssignedVariantId { get; set; }
        public int UserId { get; set; }
        public int VariantId { get; set; }
        public int ChatID { get; set; }

        public int RepoID { get; set; }

        public DateTime AssignDateTime { get; set; }
        public DateTime CompletionDateTime { get; set; }

        public DateTime DueDateTime { get; set; }//Крайний срок выполнения
        public int Mark { get; set; }

        //public int MaxMark { get; set; }

        public User User { get; set; }
        public Variant Variant { get; set; }

        //public ICollection<ChatMessages> HistoryMessages { get; set; }
    }
}