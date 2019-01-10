using System.Collections.Generic;
using System.Linq;
using JiraCloneMVC.Web.Models;
using JiraCloneMVC.Web.Repositories.Interfaces;
using System.Data.Entity;

namespace JiraCloneMVC.Web.Repositories
{
    public class GroupRepository : GenericRepository<Group>, IGroupRepository
    {
        public GroupRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        { }

        public IEnumerable<string> GetProjectRolesOfUser(string userId, int projectId)
        {
            return Entries.Include(g => g.Role).Where(g => g.ProjectId == projectId && g.UserId.Equals(userId)).Select(g => g.Role.Name).AsEnumerable();
        }
    }
}