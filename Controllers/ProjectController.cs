using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.DataAccess.Entities;
using ProjectManagement.Web.DataAccess;
using ProjectManagement.Web.Models;

namespace ProjectManagement.Web.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ProjectManagementDbContext _projectManagementDbContext;
        
        public ProjectController(ProjectManagementDbContext projectManagementDbContext)
        {
            _projectManagementDbContext = projectManagementDbContext;
        }

        [HttpGet("/projects")]
        public IActionResult GetAllProjects()
        {
            var projects = _projectManagementDbContext.Projects
                    .Include(p => p.Employees)
                    .Include(p => p.Tasks)
                    .OrderByDescending(p => p.DateCreated)
                    .ToList();

            return View("Items", projects);
        }

        [HttpGet("/projects/{id:int}")]
        public IActionResult GetProjectById(int id)
        {
            var project = _projectManagementDbContext.Projects
                .Include(p => p.Employees)
                .Include(p => p.Tasks)
                .FirstOrDefault(p => p.ProjectId == id);

            if (project == null)
                return NotFound();

            var projectDetailsViewModel = new ProjectDetailsViewModel
            {
                ActiveProject = project,
                NewEmployee = new Employee(),
                NewProjectTask = new ProjectTask(),
                CountCompletedTasks = project.Tasks.Count(t => t.TaskStatus == TaskStatusOptions.Completed),
                CountInProgressTasks = project.Tasks.Count(t => t.TaskStatus == TaskStatusOptions.InProgress),
                CountCancelledTasks = project.Tasks.Count(t => t.TaskStatus == TaskStatusOptions.Cancelled)
            };
            
            return View("Details", projectDetailsViewModel);
        }

        [Authorize]
        [HttpGet("/projects/add")]
        public IActionResult GetAddProjectRequest()
        {
            return View("AddProject", new Project());
        }

        [Authorize]
        [HttpPost("/projects/add")]
        public IActionResult AddNewProject(Project project)
        {
            if (!ModelState.IsValid) return View("AddProject", project);
            
            _projectManagementDbContext.Projects.Add(project);
            _projectManagementDbContext.SaveChanges();

            TempData["LastActionMessage"] = $"The project \"{project.Name}\" was added.";

            return RedirectToAction("GetAllProjects", "Project");

        }

        [Authorize]
        [HttpGet("/projects/{id:int}/edit")]
        public IActionResult GetEditRequestById(int id)
        {
            var project = _projectManagementDbContext.Projects.Find(id);
            return View("EditProject", project);
        }

        [Authorize]
        [HttpPost("/projects/{id:int}/edit")]
        public IActionResult ProcessEditRequest(int id, Project project)
        {
            if (!ModelState.IsValid) return View("EditProject", project);
            
            _projectManagementDbContext.Projects.Update(project);
            _projectManagementDbContext.SaveChanges();

            TempData["LastActionMessage"] = $"The project \"{project.Name}\" was updated.";

            return RedirectToAction("GetAllProjects", "Project");

        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/projects/{id:int}/delete")]
        public IActionResult GetDeleteRequestById(int id)
        {
            var project = _projectManagementDbContext.Projects.Find(id);
            return View("DeleteConfirmation", project);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("/projects/{id:int}/delete")]
        public IActionResult ProcessDeleteRequestById(int id)
        {
            var project = _projectManagementDbContext.Projects.Find(id);

            if (project == null) return RedirectToAction("GetAllProjects", "Project");
            
            _projectManagementDbContext.Projects.Remove(project);
            _projectManagementDbContext.SaveChanges();

            TempData["LastActionMessage"] = $"The project \"{project.Name}\" was deleted.";

            return RedirectToAction("GetAllProjects", "Project");
        }

        [HttpPost("/projects/{id}/employees")]
        public IActionResult AddEmployeeToProjectById(int id, ProjectDetailsViewModel projectDetailsViewModel)
        {
            Project? project = _projectManagementDbContext.Projects
                    .Include(p => p.Employees)
                    .Include(p => p.Tasks)
                    .Where(p => p.ProjectId == id)
                    .FirstOrDefault();

            if (project == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                project.Employees.Add(projectDetailsViewModel.NewEmployee);
                _projectManagementDbContext.SaveChanges();

                TempData["LastActionMessage"] = $"The employee \"{projectDetailsViewModel.NewEmployee.FullName}\" was added.";

                return RedirectToAction("GetProjectById", "Project", new { id = id });
            }
            else
            {
                projectDetailsViewModel.ActiveProject = project;
                projectDetailsViewModel.CountInProgressTasks = project.Tasks.Where(t => t.TaskStatus == TaskStatusOptions.InProgress).Count();
                projectDetailsViewModel.CountCompletedTasks = project.Tasks.Where(t => t.TaskStatus == TaskStatusOptions.Completed).Count();
                projectDetailsViewModel.CountCancelledTasks = project.Tasks.Where(t => t.TaskStatus == TaskStatusOptions.Cancelled).Count();

                return View("Details", projectDetailsViewModel);
            }
        }


        [HttpPost("/projects/{id}/tasks")]
        public IActionResult AddTaskToProjectById(int id, ProjectDetailsViewModel projectDetailsViewModel)
        {
            Project? project = _projectManagementDbContext.Projects
                    .Include(p => p.Employees)
                    .Include(p => p.Tasks)
                    .Where(p => p.ProjectId == id)
                    .FirstOrDefault();

            if (project == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                project.Tasks.Add(projectDetailsViewModel.NewProjectTask);
                _projectManagementDbContext.SaveChanges();

                TempData["LastActionMessage"] = $"The task \"{projectDetailsViewModel.NewProjectTask.Description}\" was added.";

                return RedirectToAction("GetProjectById", "Project", new { id = id });
            }
            else
            {
                projectDetailsViewModel.ActiveProject = project;
                projectDetailsViewModel.CountInProgressTasks = project.Tasks.Where(t => t.TaskStatus == TaskStatusOptions.InProgress).Count();
                projectDetailsViewModel.CountCompletedTasks = project.Tasks.Where(t => t.TaskStatus == TaskStatusOptions.Completed).Count();
                projectDetailsViewModel.CountCancelledTasks = project.Tasks.Where(t => t.TaskStatus == TaskStatusOptions.Cancelled).Count();

                return View("Details", projectDetailsViewModel);
            }
        }
    }
}
