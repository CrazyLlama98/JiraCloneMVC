using System.Collections.Generic;
using System.Linq;
using JiraCloneMVC.Web.Models;
using JiraCloneMVC.Web.Repositories.Interfaces;

namespace JiraCloneMVC.Web.Repositories
{
    public class GroupRepository : GenericRepository<Group>, IGroupRepository
    {
        public GroupRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        { }

        public IEnumerable<string> GetProjectRolesOfUser(string userId, int projectId)
        {
            return Entries.Include("Role").Where(g => g.ProjectId == projectId && g.UserId.Equals(userId)).Select(g => g.Role.Name).AsEnumerable();
        }
    }
}