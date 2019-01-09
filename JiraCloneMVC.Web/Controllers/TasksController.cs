using JiraCloneMVC.Web.Attributes;
using JiraCloneMVC.Web.Models;
using JiraCloneMVC.Web.Repositories;
using JiraCloneMVC.Web.Repositories.Interfaces;
using JiraCloneMVC.Web.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace JiraCloneMVC.Web.Controllers
{
    [RoutePrefix("projects/{projectId}/tasks"), Authorize]
    public class TasksController : Controller
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;

        public TasksController()
        {
            _taskRepository = new TaskRepository(new ApplicationDbContext());
            _projectRepository = new ProjectRepository(new ApplicationDbContext());
            _userRepository = new UserRepository(new ApplicationDbContext());
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
                    EndDate = t.EndDate,
                    ProjectId = t.ProjectId.Value
                });

            var project = _projectRepository.GetById(projectId.Value);
            if (project.OrganizerId == User.Identity.GetUserId())
                ViewBag.Role = "Organizator";
            if (User.IsInRole("Administrator"))
                ViewBag.Role = "Administrator";
            ViewBag.AdministratorRoles = new List<string>() { "Administrator", "Organizator" };
            ViewBag.ProjectId = projectId;
            return View(tasks);
        }

        [Route("{taskId}")]
        [ProjectGroupAuthorize(Roles = "Administrator,Organizator,Member", ProjectIdQueryParam = "projectId")]
        public ActionResult ViewTask(int? projectId, int? taskId)
        {
            if (!projectId.HasValue || !taskId.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var task = _taskRepository.GetById(taskId.Value);
            var project = _projectRepository.GetById(projectId.Value);
            if (project.OrganizerId == User.Identity.GetUserId())
                ViewBag.Role = "Organizator";
            if (User.IsInRole("Administrator"))
                ViewBag.Role = "Administrator";
            ViewBag.AdministratorRoles = new List<string>() { "Administrator", "Organizator" };
            return View("TaskDetails", new TaskViewModel
            {
                Assignee = task.Assignee == null ? null : new UserViewModel() { Id = task.Assignee.Id, Username = task.Assignee.UserName },
                Reporter = task.Reporter == null ? null : new UserViewModel() { Id = task.Reporter.Id, Username = task.Reporter.UserName },
                Description = task.Description,
                Title = task.Title,
                Id = task.Id,
                Status = task.Status,
                StartDate = task.StartDate,
                EndDate = task.EndDate,
                ProjectId = task.ProjectId.Value
            });
        }

        [Route("Create")]
        [ProjectGroupAuthorize(Roles = "Administrator,Organizator", ProjectIdQueryParam = "projectId")]
        public ActionResult Create(int? projectId)
        {
            if (!projectId.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            ViewBag.Users = _userRepository.GetAllFromProject(projectId.Value);
            return View("CreateTask");
        }

        [Route("Create")]
        [ProjectGroupAuthorize(Roles = "Administrator,Organizator", ProjectIdQueryParam = "projectId")]
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
                    StartDate = DateTime.Now,
                    AssigneeId = newTask.AssigneeId
                };
                _taskRepository.Add(task);
                return RedirectToAction("Index");
            }
            return View("CreateTask", newTask);
        }

        [Route("{taskId}/Edit")]
        [ProjectGroupAuthorize(Roles = "Administrator,Organizator", ProjectIdQueryParam = "projectId")]
        public ActionResult Edit(int? projectId, int? taskId)
        {
            if (!projectId.HasValue || !taskId.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var task = _taskRepository.GetById(taskId.Value);
            if (task == null)
                return HttpNotFound();
            ViewBag.Users = _userRepository.GetAllFromProject(projectId.Value);
            return View("EditTask", new CreateTaskViewModel
            {
                Description = task.Description,
                Title = task.Title,
                AssigneeId = task.AssigneeId
            });
        }

        [Route("{taskId}")]
        [ProjectGroupAuthorize(Roles = "Administrator,Organizator", ProjectIdQueryParam = "projectId")]
        [HttpPut]
        [ValidateAntiForgeryToken]
        public ActionResult Update(int? projectId, int? taskId, CreateTaskViewModel model)
        {
            if (!projectId.HasValue || !taskId.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (ModelState.IsValid)
            {
                var task = _taskRepository.GetById(taskId.Value);
                if (task == null)
                    return HttpNotFound();
                task.Title = model.Title;
                task.Description = model.Description;
                task.AssigneeId = model.AssigneeId;
                _taskRepository.Update(task);
                return RedirectToAction("ViewTask", new { projectId, taskId });
            }
            return View("EditTask", model);
        }

        [Route("{taskId}")]
        [ProjectGroupAuthorize(Roles = "Administrator,Organizator", ProjectIdQueryParam = "projectId")]
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? projectId, int? taskId)
        {
            if (!projectId.HasValue || !taskId.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var task = _taskRepository.GetById(taskId.Value);
            if (task == null)
                return HttpNotFound();
            _taskRepository.Delete(task);
            return RedirectToAction("Index");
        }

        [Route("{taskId}/status/{newStatus}")]
        [ProjectGroupAuthorize(Roles = "Administrator,Organizator,Member", ProjectIdQueryParam = "projectId")]
        [HttpPut]
        [ValidateAntiForgeryToken]
        public ActionResult EditStatus(int? projectId, int? taskId, string newStatus)
        {
            if (!projectId.HasValue || !taskId.HasValue || string.IsNullOrEmpty(newStatus))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var task = _taskRepository.GetById(taskId.Value);
            if (task == null)
                return HttpNotFound();
            task.Status = newStatus;
            if (newStatus.Equals(Constants.TaskStatus.Done))
                task.EndDate = DateTime.Now;
            _taskRepository.Update(task);
            return RedirectToAction("ViewTask", new { projectId, taskId });
        }
    }
}