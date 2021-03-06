﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JiraCloneMVC.Web.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string OrganizerId { get; set; }
        public User Organizer { get; set; }


        public ICollection<Group> Groups { get; set; }
        public ICollection<Task> Tasks { get; set; }
    }
}