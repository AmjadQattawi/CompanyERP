using CompanyERP.DTOs;
using CompanyERP.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee(EmployeeDto employeeDto)
        {
            var createdEmployee = await _employeeService.CreateEmployeeAsync(employeeDto);
            return Ok(createdEmployee);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployeeById(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            return Ok(employee);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<EmployeeDto>> UpdateEmployee(EmployeeDto employeeDto)
        {
            var updatedEmployee = await _employeeService.UpdateEmployeeAsync(employeeDto);
            return Ok(updatedEmployee);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteEmployee(int id)
        {
            await _employeeService.DeleteEmployeeAsync(id);
            return NoContent(); 
        }


        [HttpGet("{id}/preview-salary")]
        [Authorize(Roles = "Admin,Manager")] 
        public async Task<ActionResult<decimal>> CalculateSalary(int id, [FromQuery] int actualHours)
        {
            var finalSalary = await _employeeService.CalculateMonthlySalaryAsync(id, actualHours);

            return Ok(new { EmployeeId = id, ActualHoursWorked = actualHours, NetSalary = finalSalary });
        }



        [HttpPost("{id}/calculate-salary")] 
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<PayrollDto>> CalculateSalary(int id, [FromQuery] int actualHours, [FromQuery] int month, [FromQuery] int year)
        {
            var payrollResult = await _employeeService.SaveMonthlyPayrollAsync(id, actualHours, month, year);
            return Ok(payrollResult);
        }

    }
}