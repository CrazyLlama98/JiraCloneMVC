using System.Collections.Generic;
using System.Linq;
using JiraCloneMVC.Web.Models;
using JiraCloneMVC.Web.Repositories.Interfaces;
using System.Data.Entity;

namespace JiraCloneMVC.Web.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) 
            : base(dbContext)
        { }

        public IEnumerable<User> GetAllFromProject(int projectId)
        {
            return Entries
                .Include(u => u.Groups)
                .Include(u => u.Groups.Select(g => g.Role)).Where(u => u.Groups.Any(g => g.ProjectId == projectId)).AsEnumerable();
        }

        public IEnumerable<User> GetNonMembersOfProject(int projectId)
        {
            var adminRoleId = DbContext.Roles.FirstOrDefault(role => role.Name.Equals("Administrator")).Id;
            return Entries
                .Include(u => u.Groups)
                .Include(u => u.Roles)
                .Where(u => !u.Groups.Any(g => g.ProjectId == projectId) && !u.Roles.Any(r => r.RoleId.Equals(adminRoleId)));
        }

        public IEnumerable<User> GetUsersInProjectsByRole(string role, int projectId)
        {
            return Entries
                .Include(u => u.Groups)
                .Include(u => u.Groups.Select(g => g.Role))
                .Where(u => u.Groups.Any(g => g.ProjectId == projectId && g.Role.Name.Equals(role, System.StringComparison.OrdinalIgnoreCase)));
        }
    }
}