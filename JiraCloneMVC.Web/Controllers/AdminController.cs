using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JiraCloneMVC.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace JiraCloneMVC.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SeeUsers()
        {
            var users = from user in db.Users
                orderby user.UserName
                select user;
            ViewBag.UsersList = users;
            return View();
        }

        public ActionResult EditUser(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = db.Users.Find(id);

            if (user == null) return HttpNotFound();

            return View(user);
        }

        [HttpPut]
        public ActionResult EditUser(string id, string UserName, string Email, string PhoneNumber)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = db.Users.Find(id);

            if (user == null) return HttpNotFound();

            if (TryUpdateModel(user))
            {
                user.UserName = UserName;
                user.Email = Email;
                user.PhoneNumber = PhoneNumber;
                db.SaveChanges();
            }

            return RedirectToAction("SeeUsers");
        }

        public ActionResult DeleteUser(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = db.Users.Find(id);
            if (user == null) return HttpNotFound();

            int NumberOfProjects = 0;

            foreach (var proj in db.Projects.ToList())
            {
                if (proj.OrganizerId.Equals(user.Id))
                    NumberOfProjects++;
            }

            dynamic mymodel = new ExpandoObject();
            mymodel.User = user;
            mymodel.NumberOfProjects = NumberOfProjects;

            return View(mymodel);
        }

        // POST: Projects/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var user = db.Users.Find(id);
            db.Users.Remove(user);

            var groups = from gr in db.Groups
                where  gr.UserId.Equals(user.Id)
                select gr;

            foreach (var group in groups)
            {
                db.Groups.Remove(group);
            }

            db.SaveChanges();
            return RedirectToAction("SeeUsers");
        }


        public ActionResult SeeProjects()
        {
            dynamic mymodel = new ExpandoObject();

            var projects = from proj in db.Projects
                orderby proj.Name
                select proj;
            mymodel.Projects = projects;
            mymodel.Users = db.Users.ToList();
            return View(mymodel);
        }

        public ActionResult SeeTasks()
        {
            dynamic mymodel = new ExpandoObject();

            var tasks = from task in db.Tasks
                orderby task.Title
                        select task;

            mymodel.Tasks = tasks;
            mymodel.Projects = db.Projects.ToList();
            List<Project> l = db.Projects.ToList();
            var p = l.Find(x => x.Id == 1);
            mymodel.Users = db.Users.ToList();
            return View(mymodel);
        }

        public ActionResult SwitchOrganizator(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var project = db.Projects.Find(id);
            if (project == null) return HttpNotFound();

            List<User> members = new List<User>();
            foreach (var user in db.Users.ToList())
            {
                foreach (var group in db.Groups.ToList())
                {
                    if (project.Id == group.ProjectId && group.UserId == user.Id && user.Id != project.OrganizerId)
                        members.Add(user);
                }
            }

            dynamic mymodel = new ExpandoObject();
            mymodel.Project = project;
            mymodel.Organizer = db.Users.Find(project.OrganizerId);
            mymodel.Members = members;

            return View(mymodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SwitchOrganizator(int? projId, string userId)
        {
            if (projId == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var project = db.Projects.Find(projId);
            if (project == null) return HttpNotFound();

            if (userId == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = db.Users.Find(userId);
            if (user == null) return HttpNotFound();

            var groupAux = from gr in db.Groups
                where gr.UserId == userId && gr.ProjectId == projId
                select gr;

            if (groupAux.Count() != 1)
                return HttpNotFound();

            var group = groupAux.First();
            group.RoleId = db.Roles
                .FirstOrDefault(x => x.Name.Equals("Organizator", StringComparison.OrdinalIgnoreCase)).Id;

            db.Entry(group).State = EntityState.Modified; // nu functioneaza :(

            db.SaveChanges();

            return RedirectToAction("SeeProjects");
        }
        public ActionResult SeeComments()
        {
            return null;
        }
    }
}