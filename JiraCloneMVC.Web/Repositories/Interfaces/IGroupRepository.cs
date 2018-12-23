using JiraCloneMVC.Web.Models;
using System.Collections.Generic;

namespace JiraCloneMVC.Web.Repositories.Interfaces
{
    public interface IGroupRepository : IGenericRepository<Group>
    {
        IEnumerable<string> GetProjectRolesOfUser(string userId, int projectId);
    }
}
