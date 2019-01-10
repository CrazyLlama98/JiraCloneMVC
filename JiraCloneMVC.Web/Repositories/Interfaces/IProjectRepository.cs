using JiraCloneMVC.Web.Models;
using System.Collections.Generic;

namespace JiraCloneMVC.Web.Repositories.Interfaces
{
    public interface IProjectRepository : IGenericRepository<Project>
    {
        IEnumerable<Project> GetByOrganizer(string organizerId);
        IEnumerable<Project> GetByMember(string memberId);
    }
}
