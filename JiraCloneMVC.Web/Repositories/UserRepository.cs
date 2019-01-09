using System.Collections.Generic;
using System.Linq;
using JiraCloneMVC.Web.Models;
using JiraCloneMVC.Web.Repositories.Interfaces;

namespace JiraCloneMVC.Web.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) 
            : base(dbContext)
        { }

        public IEnumerable<User> GetAllFromProject(int projectId)
        {
            return Entries.Include("Groups").Where(u => u.Groups.Any(g => g.ProjectId == projectId)).AsEnumerable();
        }
    }
}