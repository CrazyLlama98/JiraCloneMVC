using JiraCloneMVC.Web.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace JiraCloneMVC.Web
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Group> Groups { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Project> Projects { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Task>().HasOptional(t => t.Reporter).WithMany().HasForeignKey(t => t.ReporterId);
            modelBuilder.Entity<Task>().HasOptional(t => t.Assignee).WithMany().HasForeignKey(t => t.AssigneeId);
            modelBuilder.Entity<Task>().HasOptional(t => t.Project).WithMany(p => p.Tasks).HasForeignKey(t => t.ProjectId);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<JiraCloneMVC.Web.Models.Comment> Comments { get; set; }
    }
}