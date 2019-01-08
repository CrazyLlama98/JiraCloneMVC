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
            return null;
        }

        public ActionResult SeeComments()
        {
            return null;
        }

        public ActionResult SwitchOrganizator(int? id)
        {
            return null;
        }
    }
}