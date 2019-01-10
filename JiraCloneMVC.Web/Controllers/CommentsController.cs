using JiraCloneMVC.Web.Attributes;
using JiraCloneMVC.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace JiraCloneMVC.Web.Controllers
{
    [RoutePrefix("projects/{projectId}/tasks/{taskId}/comments"), Authorize]
    [ProjectGroupAuthorize(Roles = "Administrator,Organizator,Member", ProjectIdQueryParam = "projectId")]
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Route]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewComment(Comment c, int? projectId, int? taskId)
        {
            try
            {
                if (!projectId.HasValue || !taskId.HasValue)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                Comment comment = new Comment
                {
                    TaskId = c.TaskId,
                    OwnerId = User.Identity.GetUserId(),
                    Content = c.Content
                };

                

                db.Comments.Add(comment);
                db.SaveChanges();

                Task task = db.Tasks.Find(c.TaskId);

                return RedirectToAction("ViewTask", "Tasks", new { projectId = task.ProjectId, taskId = c.TaskId });
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [Route("{commentId}")]
        [HttpPut]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? projectId, int? taskId, int? commentId, Comment comment)
        {
            if (!projectId.HasValue || !taskId.HasValue || !commentId.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var commentDb = db.Comments.Find(commentId);
            if (commentDb == null)
                return HttpNotFound();
            if (User.Identity.GetUserId() != commentDb.OwnerId && User.IsInRole("Member"))
                return new HttpUnauthorizedResult();
            commentDb.Content = comment.Content;
            db.Entry(comment).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ViewTask", "Tasks", new { projectId, taskId });
        }

        [Route("{commentId}")]
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? projectId, int? taskId, int? commentId)
        {
            if (!projectId.HasValue || !taskId.HasValue || !commentId.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var commentDb = db.Comments.Find(commentId);
            if (commentDb == null)
                return HttpNotFound();
            if (User.Identity.GetUserId() != commentDb.OwnerId && User.IsInRole("Member"))
                return new HttpUnauthorizedResult();
            db.Comments.Remove(commentDb);
            db.SaveChanges();
            return RedirectToAction("ViewTask", "Tasks", new { projectId, taskId });
        }
    }
}