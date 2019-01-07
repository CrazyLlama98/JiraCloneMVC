using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using JiraCloneMVC.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace JiraCloneMVC.Web.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            dynamic mymodel = new ExpandoObject();
            mymodel.UserName = User.Identity.GetUserName();

            List<Project> projectsAdministrate = new List<Project>();
            foreach (var project in db.Projects.ToList())
            {
                foreach (var group in db.Groups.ToList())
                {
                    if(group.UserId == User.Identity.GetUserId() && project.Id == group.ProjectId && 
                       project.OrganizerId == User.Identity.GetUserId())
                        projectsAdministrate.Add(project);
                }
            }
            mymodel.ProjectsAdministrate = projectsAdministrate;

            List<Project> projectsNonAdministrate = new List<Project>();
            foreach (var project in db.Projects.ToList())
            {
                foreach (var group in db.Groups.ToList())
                {
                    if (group.UserId == User.Identity.GetUserId() && project.Id == group.ProjectId &&
                        project.OrganizerId != User.Identity.GetUserId())
                        projectsNonAdministrate.Add(project);
                }
            }
            mymodel.ProjectsNonAdministrate = projectsNonAdministrate;

            mymodel.Users = db.Users.ToList();
            return View(mymodel);
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var project = db.Projects.Find(id);
            if (project == null) return HttpNotFound();
            ViewBag.OrganizerName = db.Users.Find(project.OrganizerId).UserName;
            ViewBag.Rol = "Member";
            if (project.OrganizerId == User.Identity.GetUserId())
                ViewBag.Rol = "Organizator";

            return View(project);
        }

        // GET: Projects/Details/5
        public ActionResult DetailsMembers(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var project = db.Projects.Find(id);
            if (project == null) return HttpNotFound();

            dynamic mymodel = new ExpandoObject();
            mymodel.Project = project;
            mymodel.Organizer = db.Users.Find(project.OrganizerId);

            List<User> members = new List<User>();
            foreach (var user in db.Users.ToList())
            {
                foreach (var group in db.Groups.ToList())
                {
                    if (project.Id == group.ProjectId && group.UserId == user.Id && user.Id != project.OrganizerId)
                        members.Add(user);
                }
            }
            mymodel.Members = members;
            ViewBag.Rol = "Member";
            if (project.OrganizerId == User.Identity.GetUserId())
                ViewBag.Rol = "Organizator";

            return View(mymodel);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Status,StartDate,EndDate")]
            Project project)
        {
            if (ModelState.IsValid)
            {
                project.OrganizerId = User.Identity.GetUserId();

                Group group = new Group();
                group.UserId = project.OrganizerId;
                group.ProjectId = project.Id;
                group.RoleId = db.Roles
                    .FirstOrDefault(x => x.Name.Equals("Organizator", StringComparison.OrdinalIgnoreCase)).Id;
                db.Groups.Add(group);

                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Create
        public ActionResult AddMember(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var project = db.Projects.Find(id);
            if (project == null) return HttpNotFound();

            dynamic mymodel = new ExpandoObject();
            mymodel.Project = project;
            mymodel.Organizer = db.Users.Find(project.OrganizerId);

            List<User> members = new List<User>();
            foreach (var user in db.Users.ToList())
            {
                bool isMember = false;
                foreach (var group in db.Groups.ToList())
                {
                    if (project.Id == group.ProjectId && group.UserId == user.Id)
                        isMember = true;
                }
                if (!isMember)
                    members.Add(user);
            }

            mymodel.Users = members;
            return View(mymodel);
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMember( int? idProj, string idUser)
        {

            if (idProj == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var project = db.Projects.Find(idProj);
            if (project == null) return HttpNotFound();

            if (idUser == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = db.Users.Find(idUser);
            if (user == null) return HttpNotFound();

            Group group = new Group();
            group.UserId = user.Id;
            group.ProjectId = project.Id;
            group.RoleId = db.Roles
                .FirstOrDefault(x => x.Name.Equals("Member", StringComparison.OrdinalIgnoreCase)).Id;
            db.Groups.Add(group);

            db.SaveChanges();

            return RedirectToAction("DetailsMembers", new {id = idProj});
        }

        // GET: Projects/Edit/5
        //[Authorize(Roles = "Organizator,Administrator")]
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var project = db.Projects.Find(id);

            if (project == null) return HttpNotFound();

            if (project.OrganizerId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
                return View(project);

            TempData["message"] = "You do not have the right to make changes to " +
                                  "a PROJECT that does not belong to you!";
            return RedirectToAction("Index");
        }

        // POST: Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Organizator,Administrator")]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Status,StartDate,EndDate")]
            Project project)
        {
            if (ModelState.IsValid)
            {
                if (project.OrganizerId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
                {
                    db.Entry(project).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                TempData["message"] = "You do not have the right to make changes to " +
                                      "a PROJECT that does not belong to you!";
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Delete/5
        //[Authorize(Roles = "Organizator,Administrator")]
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var project = db.Projects.Find(id);
            if (project == null) return HttpNotFound();

            if (project.OrganizerId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
                return View(project);

            TempData["message"] = "You do not have the right to make changes to " +
                                  "a PROJECT that does not belong to you!";
            return RedirectToAction("Index");
        }

        // POST: Projects/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Organizator,Administrator")]
        public ActionResult DeleteConfirmed(int id)
        {
            var project = db.Projects.Find(id);
            if (project.OrganizerId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                db.Projects.Remove(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            TempData["message"] = "You do not have the right to make changes to " +
                                  "a PROJECT that does not belong to you!";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();

            base.Dispose(disposing);
        }

        [HttpPost]
        [ActionName("DeleteMember")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Organizator,Administrator")]
        public ActionResult DeleteMember(string userId, int? projId)
        {
            if (userId == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (projId == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Group gr = null;

            foreach (var grup in db.Groups.ToList())
            {
                if (grup.UserId == userId && grup.ProjectId == projId)
                {
                    gr = grup;
                    break;
                }

            }
            if (gr == null) return HttpNotFound();

            db.Groups.Remove(gr);
            db.SaveChanges();

            return RedirectToAction("DetailsMembers", new { id = projId });
        }
    }
}