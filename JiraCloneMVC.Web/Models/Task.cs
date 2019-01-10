using System;
using System.Collections.Generic;

namespace JiraCloneMVC.Web.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int? ProjectId { get; set; }
        public Project Project { get; set; }
        public string AssigneeId { get; set; }
        public User Assignee { get; set; }
        public string ReporterId { get; set; }
        public User Reporter { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}