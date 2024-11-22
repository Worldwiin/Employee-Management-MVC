using System.Collections.Generic;
using Employee_Management_MVC.Models;

namespace Employee_Management_MVC.ViewModels
{
    public class EmployeeListViewModel
    {
        public List<Employee> Employees { get; set; } = new List<Employee>(); 
        public int PageNumber { get; set; } 
        public int PageSize { get; set; } 
        public int TotalPages { get; set; } 

        public string SearchTerm { get; set; } 
        public string SelectedDepartment { get; set; } 
        public string SelectedType { get; set; } 
    }
}