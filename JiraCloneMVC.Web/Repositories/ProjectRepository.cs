using JiraCloneMVC.Web.Models;
using JiraCloneMVC.Web.Repositories.Interfaces;

namespace JiraCloneMVC.Web.Repositories
{
    public class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
        public ProjectRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        { }
    }
}