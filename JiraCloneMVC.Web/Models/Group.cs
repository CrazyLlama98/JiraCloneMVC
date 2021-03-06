﻿using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace JiraCloneMVC.Web.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; } 
        public string RoleId { get; set; }
        public int ProjectId { get; set; }

        public User User { get; set; }
        public IdentityRole Role { get; set; }
        public Project Project { get; set; }
    }
}