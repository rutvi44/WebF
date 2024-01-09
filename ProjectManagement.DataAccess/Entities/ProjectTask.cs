namespace ProjectManagement.DataAccess.Entities
{
    public enum TaskStatusOptions { InProgress, Completed, Cancelled };

    public class ProjectTask
    {
        public int ProjectTaskId { get; set; }

        public DateTime? DueDate { get; set; }

        public string? Description { get; set; }

        public TaskStatusOptions TaskStatus { get; set; } = TaskStatusOptions.InProgress;

        // FK:
        public int? ProjectId { get; set; }

        // And nav prop:
        public Project? Project { get; set; }
    }
}
