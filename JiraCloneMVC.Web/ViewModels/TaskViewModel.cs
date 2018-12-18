using System;

namespace JiraCloneMVC.Web.ViewModels
{
    public class TaskViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public UserViewModel Assignee { get; set; }
        public UserViewModel Reporter { get; set; }
    }
}