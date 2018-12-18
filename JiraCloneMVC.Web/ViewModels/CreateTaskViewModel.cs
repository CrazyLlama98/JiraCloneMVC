using System.ComponentModel.DataAnnotations;

namespace JiraCloneMVC.Web.ViewModels
{
    public class CreateTaskViewModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
    }
}