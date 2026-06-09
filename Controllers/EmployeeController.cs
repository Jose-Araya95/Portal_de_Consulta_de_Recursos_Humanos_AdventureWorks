using Microsoft.AspNetCore.Mvc;
using Portal_de_Consulta_de_Recursos_Humanos_AdventureWorks.DAL;
using Portal_de_Consulta_de_Recursos_Humanos_AdventureWorks.Models;

namespace Portal_de_Consulta_de_Recursos_Humanos_AdventureWorks.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeDAL _employeeDal;

        // Dependency Injection: ASP.NET automatically provides the EmployeeDAL 
        // we registered in Program.cs
        public EmployeeController(EmployeeDAL employeeDal)
        {
            _employeeDal = employeeDal;
        }

        // The Index action handles the initial page load AND the form submissions.
        // It accepts the filter parameters directly from the HTML form's GET request.
        public IActionResult Index(string name, int? departmentId, string jobTitle, int? shiftId, bool onlyActive = true)
        {
            // We pass the filter values back to the view via ViewBag so the form 
            // remembers what the user typed in after the page reloads.
            ViewBag.CurrentName = name;
            ViewBag.CurrentDepartmentId = departmentId;
            ViewBag.CurrentJobTitle = jobTitle;
            ViewBag.CurrentShiftId = shiftId;
            ViewBag.CurrentOnlyActive = onlyActive;

            // Call the DAL to execute the dynamic SQL query
            List<EmployeeViewModel> employees = _employeeDal.GetEmployees(name, departmentId, jobTitle, shiftId, onlyActive);

            // Pass the populated list of employees to the Razor View
            return View(employees);
        }
    }
}