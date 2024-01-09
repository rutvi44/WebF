using ProjectManagement.DataAccess.Entities;

namespace ProjectManagement.Web.Components
{
    public class TasksViewModel
    {
        public List<ProjectTask>? Tasks { get; set; }
        public int NumberOfTasksToDisplay { get; set; }
    }
}
