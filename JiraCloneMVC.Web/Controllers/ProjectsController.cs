using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using JiraCloneMVC.Web.Attributes;
using JiraCloneMVC.Web.Models;
using JiraCloneMVC.Web.Repositories;
using JiraCloneMVC.Web.Repositories.Interfaces;
using JiraCloneMVC.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace JiraCloneMVC.Web.Controllers
{
    [RoutePrefix("projects"), Authorize]
    public class ProjectsController : Controller
    {
        private IProjectRepository _projectRepository = new ProjectRepository(new ApplicationDbContext());
        private IUserRepository _userRepository = new UserRepository(new ApplicationDbContext());
        private IRoleRepository _roleRepository = new RoleRepository(new ApplicationDbContext());
        private IGroupRepository _groupRepository = new GroupRepository(new ApplicationDbContext());

        // GET: Projects
        [Route]
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            dynamic mymodel = new ExpandoObject();
            mymodel.UserName = User.Identity.GetUserName();
            if (User.IsInRole("Administrator"))
            {
                mymodel.ProjectsAdministrate = _projectRepository.GetAll();
                mymodel.ProjectsNonAdministrate = new List<Project>();
            }
            else {
                mymodel.ProjectsAdministrate = _projectRepository.GetByOrganizer(User.Identity.GetUserId());
                mymodel.ProjectsNonAdministrate = _projectRepository.GetByMember(User.Identity.GetUserId());
                mymodel.Users = _userRepository.GetAll();
            }
            return View(mymodel);
        }

        // GET: Projects/Details/5
        [Route("{id}/details")]
        [ProjectGroupAuthorize(Roles = "Administrator,Organizator,Member", ProjectIdQueryParam = "id")]
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var project = _projectRepository.GetById(id);
            if (project == null) return HttpNotFound();
            ViewBag.OrganizerName = _userRepository.GetById(project.OrganizerId).UserName;
            ViewBag.Rol = "Member";
            if (project.OrganizerId == User.Identity.GetUserId())
                ViewBag.Rol = "Organizator";

            if (User.IsInRole("Administrator"))
                ViewBag.Rol = "Administrator";

            return View(project);
        }

        // GET: Projects/Details/5
        [Route("{id}/detailsmembers")]
        [ProjectGroupAuthorize(Roles = "Administrator,Organizator,Member", ProjectIdQueryParam = "id")]
        public ActionResult DetailsMembers(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var project = _projectRepository.GetById(id);
            if (project == null) return HttpNotFound();

            dynamic mymodel = new ExpandoObject();
            mymodel.Project = project;
            
            mymodel.Members = _userRepository.GetAllFromProject(project.Id);
            ViewBag.Rol = "Member";
            if (project.OrganizerId == User.Identity.GetUserId())
                ViewBag.Rol = "Organizator";

            if (User.IsInRole("Administrator"))
                ViewBag.Rol = "Administrator";

            return View(mymodel);
        }

        // GET: Projects/Create
        [Route("create")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateProjectViewModel newProject)
        {
            if (ModelState.IsValid)
            {
                var project = new Project { Name = newProject.Name, Description = newProject.Description };
                project.OrganizerId = User.Identity.GetUserId();
                project.StartDate = DateTime.Now;
                project.Status = Constants.ProjectStatus.Open;
                _projectRepository.Add(project);
                Group group = new Group
                {
                    UserId = project.OrganizerId,
                    ProjectId = project.Id,
                    RoleId = _roleRepository.FirstOrDefault(x => x.Name.Equals("Organizator", StringComparison.OrdinalIgnoreCase)).Id
                };
                _groupRepository.Add(group);
                return RedirectToAction("Index");
            }
            return View(newProject);
        }

        [Route("{id}/AddMember")]
        [ProjectGroupAuthorize(Roles = "Administrator,Organizator", ProjectIdQueryParam = "id")]
        public ActionResult AddMember(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var project = _projectRepository.GetById(id);
            if (project == null) return HttpNotFound();

            dynamic mymodel = new ExpandoObject();
            mymodel.Project = project;
            mymodel.Organizers = _userRepository.GetUsersInProjectsByRole("Organizator", project.Id);

            mymodel.Users = _userRepository.GetNonMembersOfProject(project.Id);
            return View(mymodel);
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{idProj}/addmember/{idUser}")]
        [ProjectGroupAuthorize(Roles = "Administrator,Organizator", ProjectIdQueryParam = "idProj")]
        public ActionResult AddMember( int? idProj, string idUser)
        {
            if (idProj == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var project = _projectRepository.GetById(idProj);
            if (project == null) return HttpNotFound();

            if (idUser == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = _userRepository.GetById(idUser);
            if (user == null) return HttpNotFound();

            Group group = new Group
            {
                UserId = user.Id,
                ProjectId = project.Id,
                RoleId = _roleRepository.FirstOrDefault(x => x.Name.Equals("Member", StringComparison.OrdinalIgnoreCase)).Id
            };
            _groupRepository.Add(group);

            return RedirectToAction("DetailsMembers", new {id = idProj});
        }

        [Route("{id}/edit")]
        [ProjectGroupAuthorize(Roles = "Administrator,Organizator", ProjectIdQueryParam = "id")]
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var project = _projectRepository.GetById(id);

            if (project == null) return HttpNotFound();

            if (project.OrganizerId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
                return View(new EditProjectViewModel { Id = project.Id, Name = project.Name, Description = project.Description, Status = project.Status });

            TempData["message"] = "You do not have the right to make changes to " +
                                  "a PROJECT that does not belong to you!";
            return RedirectToAction("Index");
        }

        [Route("{id}")]
        [HttpPut]
        [ValidateAntiForgeryToken]
        [ProjectGroupAuthorize(Roles = "Administrator,Organizator", ProjectIdQueryParam = "id")]
        public ActionResult Update(EditProjectViewModel project, int id)
        {
            if (ModelState.IsValid)
            {
                var projectDb = _projectRepository.GetById(id);
                if (projectDb.OrganizerId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
                {
                    if (project.Status.Equals(Constants.ProjectStatus.Closed))
                        projectDb.EndDate = DateTime.Now;
                    else
                        projectDb.EndDate = null;
                    projectDb.Name = project.Name;
                    projectDb.Description = project.Description;
                    projectDb.Status = project.Status;
                    _projectRepository.Update(projectDb);
                    return RedirectToAction("Index");
                }

                TempData["message"] = "You do not have the right to make changes to " +
                                      "a PROJECT that does not belong to you!";
                return RedirectToAction("Index");
            }

            return View(project);
        }

        [Route("{id}/delete")]
        [ProjectGroupAuthorize(Roles = "Administrator,Organizator", ProjectIdQueryParam = "id")]
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var project = _projectRepository.GetById(id);
            if (project == null) return HttpNotFound();

            if (project.OrganizerId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
                return View(project);

            TempData["message"] = "You do not have the right to make changes to " +
                                  "a PROJECT that does not belong to you!";
            return RedirectToAction("Index");
        }

        // POST: Projects/Delete/5
        [HttpDelete]
        [ValidateAntiForgeryToken]
        [Route("{id}")]
        [ProjectGroupAuthorize(Roles = "Administrator,Organizator", ProjectIdQueryParam = "id")]
        public ActionResult DeleteConfirmed(int id)
        {
            var project = _projectRepository.GetById(id);
            if (project.OrganizerId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                _projectRepository.Delete(project);
                return RedirectToAction("Index");
            }

            TempData["message"] = "You do not have the right to make changes to " +
                                  "a PROJECT that does not belong to you!";
            return RedirectToAction("Index");
        }

        [HttpDelete]
        [ActionName("DeleteMember")]
        [ValidateAntiForgeryToken]
        [Route("{projId}/members/{userId}")]
        [ProjectGroupAuthorize(Roles = "Administrator,Organizator", ProjectIdQueryParam = "projId")]
        public ActionResult DeleteMember(string userId, int? projId)
        {
            if (userId == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (projId == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var group = _groupRepository.FirstOrDefault(g => g.UserId == userId && g.ProjectId == projId);
            if (group == null) return HttpNotFound();

            _groupRepository.Delete(group);

            return RedirectToAction("DetailsMembers", new { id = projId });
        }
    }
}