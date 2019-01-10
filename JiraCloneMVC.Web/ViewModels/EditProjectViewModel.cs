namespace JiraCloneMVC.Web.ViewModels
{
    public class EditProjectViewModel : CreateProjectViewModel
    {
        public int Id { get; set; }
        public string Status { get; set; }
    }
}