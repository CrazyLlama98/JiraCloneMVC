﻿using System;
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
            List<Project> projects = new List<Project>();
            foreach (var project in db.Projects.ToList())
            {
                foreach (var group in db.Groups.ToList())
                {
                    if(group.UserId == User.Identity.GetUserId() && project.Id == group.ProjectId)
                        projects.Add(project);
                }
            }

            mymodel.Projects = projects;
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

            return View(project);
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
    }
}