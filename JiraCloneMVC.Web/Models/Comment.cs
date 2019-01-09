using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JiraCloneMVC.Web.Models
{
    public class Comment
    {
        public int Id { get; set; }
        [Required]
        public string Content { get; set; }

        public string OwnerId { get; set; }
        public int TaskId { get; set; }

        public User Owner { get; set; }
        public Task Task { get; set; }

    }
}