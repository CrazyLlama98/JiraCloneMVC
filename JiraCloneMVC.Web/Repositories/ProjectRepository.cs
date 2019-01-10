using System.Collections.Generic;
using System.Linq;
using JiraCloneMVC.Web.Models;
using JiraCloneMVC.Web.Repositories.Interfaces;
using System.Data.Entity;

namespace JiraCloneMVC.Web.Repositories
{
    public class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
        public ProjectRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        { }

        public IEnumerable<Project> GetByMember(string memberId)
        {
            return Entries
                .Include(p => p.Groups.Select(g => g.Role))
                .Include(p => p.Organizer)
                .Where(p => p.Groups
                    .Any(g => g.UserId.Equals(memberId) & 
                        g.Role.Name.Equals("Member", System.StringComparison.OrdinalIgnoreCase)));
        }

        public IEnumerable<Project> GetByOrganizer(string organizerId)
        {
            return Entries
                .Include(p => p.Groups.Select(g => g.Role))
                .Include(p => p.Organizer)
                .Where(p => p.Groups
                    .Any(g => g.UserId.Equals(organizerId) && 
                        g.Role.Name.Equals("Organizator", System.StringComparison.OrdinalIgnoreCase)));
        }

        public override IEnumerable<Project> GetAll()
        {
            return Entries.Include(p => p.Organizer).AsEnumerable();
        }

        public override Project GetById(object id)
        {
            return Entries.Include(p => p.Organizer).FirstOrDefault(p => p.Id == (int) id);
        }
    }
}