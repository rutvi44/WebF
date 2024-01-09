using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.DataAccess.Entities
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Employee Number is required")]
        [RegularExpression(@"^[a-zA-Z]{3}-\d{6}$", ErrorMessage = "EmployeeNumber must follow the format of 3 letters (case insensitive), a dash, and 6 digits")]
        public string? EmployeeNumber { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string? LastName { get; set; }

        public string? FullName => $"{LastName}, {FirstName}";

        // FK:
        public int? ProjectId { get; set; }

        // And nav prop:
        public Project? Project { get; set; }
    }
}
