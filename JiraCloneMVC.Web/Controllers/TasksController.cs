using JiraCloneMVC.Web.Models;
using JiraCloneMVC.Web.Repositories;
using JiraCloneMVC.Web.Repositories.Interfaces;
using JiraCloneMVC.Web.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace JiraCloneMVC.Web.Controllers
{
    [RoutePrefix("projects/{projectId}/tasks"), Authorize]
    public class TasksController : Controller
    {
        private readonly ITaskRepository _taskRepository;

        public TasksController()
        {
            _taskRepository = new TaskRepository(new ApplicationDbContext());
        }

        [Route]
        public ActionResult Index(int? projectId)
        {
            if (!projectId.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var tasks = _taskRepository.GetByProjectId(projectId.Value)
                .Select(t => new TaskViewModel()
                {
                    Assignee = t.Assignee == null ? null : new UserViewModel() { Id = t.Assignee.Id, Username = t.Assignee.UserName },
                    Reporter = t.Reporter == null ? null : new UserViewModel() { Id = t.Reporter.Id, Username = t.Reporter.UserName },
                    Description = t.Description,
                    Title = t.Title,
                    Id = t.Id,
                    Status = t.Status,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate
                });
            return View(tasks);
        }

        [Route("{taskId}")]
        public ActionResult ViewTask(int? projectId, int? taskId)
        {
            if (!projectId.HasValue || !taskId.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var task = _taskRepository.GetById(taskId);
            return View("TaskDetails", new TaskViewModel
            {
                Assignee = task.Assignee == null ? null : new UserViewModel() { Id = task.Assignee.Id, Username = task.Assignee.UserName },
                Reporter = task.Reporter == null ? null : new UserViewModel() { Id = task.Reporter.Id, Username = task.Reporter.UserName },
                Description = task.Description,
                Title = task.Title,
                Id = task.Id,
                Status = task.Status,
                StartDate = task.StartDate,
                EndDate = task.EndDate
            });
        }

        [Route("Create")]
        public ActionResult Create(int? projectId)
        {
            if (!projectId.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            return View("CreateTask");
        }

        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int? projectId, CreateTaskViewModel newTask)
        {
            if (!projectId.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (ModelState.IsValid)
            {
                var task = new Task
                {
                    Description = newTask.Description,
                    ReporterId = User.Identity.GetUserId(),
                    ProjectId = projectId.Value,
                    Status = Constants.TaskStatus.ToDo,
                    Title = newTask.Title,
                    StartDate = DateTime.Now
                };
                _taskRepository.Add(task);
                return RedirectToAction("Index");
            }
            return View("CreateTask");
        }
    }
}