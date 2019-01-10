using System.ComponentModel.DataAnnotations;

namespace JiraCloneMVC.Web.ViewModels
{
    public class CreateProjectViewModel
    {
        [Required]
        [StringLength(30, ErrorMessage = "The length of the name should have maximum 30 characters")]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }
}