using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JiraCloneMVC.Web.Models
{
    public class Project
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public  String Description { get; set; }
        public String Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public String OrganizerId { get; set; }
        public User Organizer { get; set; }


        public ICollection<Group> Groups { get; set; }
    }
}