using System.ComponentModel.DataAnnotations;
using System;
using LMS.EntityСontext;

namespace LMS.DTO
{
    public class RepositoryDTO
    {
        [Required]
        public int UserId { get; set; }

        public string UserName { get; set; } //т.к поле в бд не уникальное возможно не нужно
        public string Name { get; set; }
        public string Description { get; set; }
        public string DefaultBranch { get; set; }

        //public int NumIssues { get; set; }
        //public int NumOpenIssues { get; set; }
        //public int NumPulls { get; set; }
        //public int NumOpenPulls { get; set; }
        public DateTime CreationDate { get; set; }

        //public bool IsPrivate { get; set; }
        //public bool IsMirror { get; set; }
        public long Size { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}