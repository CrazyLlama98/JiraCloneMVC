using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JiraCloneMVC.Web;
using JiraCloneMVC.Web.Models;
using Microsoft.AspNet.Identity;

namespace JiraCloneMVC.Web.Controllers
{
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        public ActionResult Index()
        {
            var comments = db.Comments.Include(c => c.Owner).Include(c => c.Task);
            return View(comments.ToList());
        }

        // GET: Comments/Create
        public ActionResult Create()
        {
            ViewBag.OwnerId = User.Identity.GetUserId();
            ViewBag.TaskId = new SelectList(db.Tasks, "Id", "Title");
            return View();
        }

        // POST: Comments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Content,OwnerId,TaskId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OwnerId = User.Identity.GetUserId();
            ViewBag.TaskId = new SelectList(db.Tasks, "Id", "Title", comment.TaskId);
            return View(comment);
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.OwnerId = User.Identity.GetUserId();
            ViewBag.TaskId = new SelectList(db.Tasks, "Id", "Title", comment.TaskId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        [HttpPut]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Content,OwnerId,TaskId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OwnerId = User.Identity.GetUserId();
            ViewBag.TaskId = new SelectList(db.Tasks, "Id", "Title", comment.TaskId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
