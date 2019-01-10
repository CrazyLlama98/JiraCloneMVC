using JiraCloneMVC.Web.Repositories.Interfaces;
using Microsoft.AspNet.Identity.EntityFramework;

namespace JiraCloneMVC.Web.Repositories
{
    public class RoleRepository : GenericRepository<IdentityRole>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext dbContext) 
            : base(dbContext)
        { }
    }
}