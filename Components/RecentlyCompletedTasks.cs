using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.DataAccess.Entities;
using ProjectManagement.Web.DataAccess;

namespace ProjectManagement.Web.Components
{
    public class RecentlyCompletedTasks : ViewComponent
    {
        private readonly ProjectManagementDbContext _projectManagementDbContext;
        
        public RecentlyCompletedTasks(ProjectManagementDbContext projectManagementDbContext)
        {
            _projectManagementDbContext = projectManagementDbContext;
        }

        public IViewComponentResult Invoke(int numberOfTasksToDisplay)
        {
            var projectTasks = _projectManagementDbContext.ProjectTasks
                    .Include(pt => pt.Project)
                    .Where(pt => pt.DueDate < DateTime.Now && pt.TaskStatus == TaskStatusOptions.Completed)
                    .OrderByDescending(pt => pt.DueDate)
                    .ToList();

            var recentlyCompletedTasksViewModel = new TasksViewModel {
                Tasks = projectTasks,
                NumberOfTasksToDisplay = numberOfTasksToDisplay
            };

            return View(recentlyCompletedTasksViewModel);
        }
    }
}
