using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SignalR.Models
{
    public partial class Employee
    {
        public Employee()
        {
            Accounts = new HashSet<Account>();
            Orders = new HashSet<Order>();
        }

        public int EmployeeId { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; } = null!;
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; } = null!;
        [Required(ErrorMessage = "Department is required")]
        public int? DepartmentId { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string? Title { get; set; }
        [Required(ErrorMessage = "Title of courtesy is required")]
        public string? TitleOfCourtesy { get; set; }
        [Required(ErrorMessage = "Birthdate is required")]
        public DateTime? BirthDate { get; set; }
        public DateTime? HireDate { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string? Address { get; set; }

        public virtual Department? Department { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
