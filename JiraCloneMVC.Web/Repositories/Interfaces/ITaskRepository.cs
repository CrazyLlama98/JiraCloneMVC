using JiraCloneMVC.Web.Models;
using System.Collections.Generic;

namespace JiraCloneMVC.Web.Repositories.Interfaces
{
    public interface ITaskRepository : IGenericRepository<Task>
    {
        IEnumerable<Task> GetByProjectId(int projectId);
    }
}
