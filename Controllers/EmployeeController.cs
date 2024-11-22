using Employee_Management_MVC.Models;
using Employee_Management_MVC.Services;
using Employee_Management_MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Employee_Management_MVC.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeService _employeeService;

        public EmployeeController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> List(
            [FromQuery] string SearchTerm, 
            [FromQuery] string SelectedDepartment, 
            [FromQuery] string SelectedType, 
            [FromQuery] int PageNumber = 1, 
            [FromQuery] int PageSize = 5)
        {
            var (employees, totalCount) = await _employeeService.GetEmployees(SearchTerm, SelectedDepartment, SelectedType, PageNumber, PageSize);

            var viewModel = new EmployeeListViewModel
            {
                Employees = employees,
                PageNumber = PageNumber,
                PageSize = PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / PageSize),
                SearchTerm = SearchTerm,
                SelectedDepartment = SelectedDepartment,
                SelectedType = SelectedType
            };

            GetSelectLists();
            ViewBag.PageSizeOptions = new SelectList(new List<int> { 3, 5, 10, 15, 20, 25 }, PageSize);

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            GetSelectLists();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.DepartmentOptions = new SelectList(Enum.GetValues(typeof(Department)));
                ViewBag.EmployeeTypeOptions = new SelectList(Enum.GetValues(typeof(EmployeeType)));
                return View(employee);
            }

            try
            {
                await _employeeService.CreateEmployee(employee);
                return RedirectToAction("Success", employee);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the employee.");
                return View(employee);
            }
        }

        public async Task<IActionResult> Success([FromRoute] int id) 
        {
            var employee = await _employeeService.GetEmployeeById(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        public async Task<IActionResult> Details(int id)
        {
            var employee = await _employeeService.GetEmployeeById(id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpGet]
        public async Task<IActionResult> Update([FromRoute] int id) 
        {
            var employee = await _employeeService.GetEmployeeById(id);
            if (employee == null) return NotFound();

            GetSelectLists();
            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromForm] Employee employee) 
        {
            if (ModelState.IsValid)
            {
                await _employeeService.UpdateEmployee(employee);
                TempData["Message"] = $"Employee with ID {employee.Id} and Name {employee.FullName} has been updated.";
                return RedirectToAction("List");
            }
            GetSelectLists();
            return View(employee);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _employeeService.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _employeeService.DeleteEmployee(id);
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while deleting the employee.");
                return RedirectToAction("Delete", new { id });
            }
        }

        [HttpGet]
        public JsonResult GetPositions(Department department) 
        {
            var positions = new Dictionary<Department, List<string>>
            {
                { Department.IT, new List<string> { "Software Developer", "System Administrator", "Network Engineer" } },
                { Department.HR, new List<string> { "HR Specialist", "HR Manager", "Talent Acquisition Coordinator" } },
                { Department.Sales, new List<string> { "Sales Executive", "Sales Manager", "Account Executive" } },
                { Department.Admin, new List<string> { "Office Manager", "Executive Assistant", "Receptionist" } }
            };

            var result = positions.ContainsKey(department) ? positions[department] : new List<string>();
            return Json(result);
        }

        private void GetSelectLists()
        {
            ViewBag.DepartmentOptions = new SelectList(Enum.GetValues(typeof(Department)).Cast<Department>());
            ViewBag.EmployeeTypeOptions = new SelectList(Enum.GetValues(typeof(EmployeeType)).Cast<EmployeeType>());
        }
    }
}
