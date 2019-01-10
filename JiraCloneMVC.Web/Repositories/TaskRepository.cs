using System.Collections.Generic;
using System.Linq;
using JiraCloneMVC.Web.Models;
using JiraCloneMVC.Web.Repositories.Interfaces;
using System.Data.Entity;

namespace JiraCloneMVC.Web.Repositories
{
    public class TaskRepository : GenericRepository<Task>, ITaskRepository
    {
        public TaskRepository(ApplicationDbContext dbContext) 
            : base(dbContext)
        { }

        public IEnumerable<Task> GetByProjectId(int projectId)
        {
            return Entries.Include("Assignee").Include("Reporter").Where(t => t.ProjectId == projectId).AsEnumerable();
        }

        public override Task GetById(object id)
        {
            return Entries
                .Include("Assignee")
                .Include("Reporter")
                .Include(t => t.Comments)
                .Include(t => t.Comments.Select(c => c.Owner))
                .FirstOrDefault(t => t.Id == (int) id);
        }
    }
}