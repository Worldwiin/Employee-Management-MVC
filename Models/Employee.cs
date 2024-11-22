namespace Employee_Management_MVC.Models;

public enum Department
{
    IT,
    HR,
    Sales,
    Admin
}

public enum EmployeeType
{
    FullTime,
    PartTime,
    Intern,
    Contract,
    Temporary,
    Permanent
}

public class Employee
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public Department Department { get; set; }
    public string Position { get; set; }
    public EmployeeType Type { get; set; }
    public DateTime? HireDate { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Gender { get; set; }
    public decimal? Salary { get; set; }
}