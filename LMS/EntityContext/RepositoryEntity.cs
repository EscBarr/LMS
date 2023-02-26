using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace LMS.EntityСontext
{
    public class RepositoryEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DefaultBranch { get; set; }

        //public int NumIssues { get; set; }
        //public int NumOpenIssues { get; set; }
        //public int NumPulls { get; set; }
        //public int NumOpenPulls { get; set; }
        public DateTime CreationDate { get; set; }

        //public bool IsPrivate { get; set; } //
        //public bool IsMirror { get; set; }
        public long Size { get; set; }

        public DateTime UpdateTime { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } //Для генерации ссылок на репозиторий лучше использовать ник пользователя

        public User User { get; set; }
    }
}