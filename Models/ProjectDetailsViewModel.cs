using ProjectManagement.DataAccess.Entities;
namespace ProjectManagement.Web.Models
{
    public class ProjectDetailsViewModel
    {
        public Project? ActiveProject { get; set; }
        public Employee? NewEmployee{get; set;}
        public ProjectTask? NewProjectTask { get; set; }
        public int CountCompletedTasks{get; set;} = 0;
        public int CountInProgressTasks{get; set;} = 0;
        public int CountCancelledTasks{get; set;} = 0;


       // public ProjectDetailsViewModel() { NewProjectTask = new ProjectTask(); }
    }
}
