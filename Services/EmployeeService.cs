using Employee_Management_MVC.Models;
using Microsoft.EntityFrameworkCore;


namespace Employee_Management_MVC.Services
{
    public class EmployeeService
    {
        private readonly EmployeeContext _context;

        public EmployeeService(EmployeeContext context)
        {
            _context = context;
        }

        public async Task<(List<Employee> Employees, int TotalCount)> GetEmployees(
            string SearchTerm,
            string SelectedDepartment,
            string SelectedType,
            int PageNumber,
            int PageSize)
        {
            var filteredEmployees = _context.Employees.AsQueryable();

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                filteredEmployees = filteredEmployees.Where(p =>
                    p.FullName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(SelectedDepartment))
            {
                if (Enum.TryParse(SelectedDepartment, out Department department))
                {
                    filteredEmployees = filteredEmployees.Where(p => p.Department == department);
                }
            }

            if (!string.IsNullOrEmpty(SelectedType))
            {
                if (Enum.TryParse(SelectedType, out EmployeeType type))
                {
                    filteredEmployees = filteredEmployees.Where(p => p.Type == type);
                }
            }

            int totalCount = await filteredEmployees.CountAsync();
            filteredEmployees = filteredEmployees.Skip((PageNumber - 1) * PageSize).Take(PageSize);

            var employeesList = await filteredEmployees.ToListAsync();
            return (employeesList, totalCount);
        }

        public async Task<Employee?> GetEmployeeById(int id)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task CreateEmployee(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEmployee(Employee employee)
        {
            var existingEmployee = await GetEmployeeById(employee.Id);
            if (existingEmployee != null)
            {
                existingEmployee.FullName = employee.FullName;
                existingEmployee.Email = employee.Email;
                existingEmployee.Position = employee.Position;
                existingEmployee.Department = employee.Department;
                existingEmployee.HireDate = employee.HireDate;
                existingEmployee.DateOfBirth = employee.DateOfBirth;
                existingEmployee.Type = employee.Type;
                existingEmployee.Gender = employee.Gender;
                existingEmployee.Salary = employee.Salary;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteEmployee(int id)
        {
            var employee = await GetEmployeeById(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }
    }
}