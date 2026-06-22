using CompanyERP.DTOs;

namespace CompanyERP.IServices
{
    public interface IEmployeeService
    {

        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
        Task<EmployeeDto> CreateEmployeeAsync(EmployeeDto employeeDto);
        Task<EmployeeDto> GetEmployeeByIdAsync(int id);
        Task<EmployeeDto> UpdateEmployeeAsync(EmployeeDto employeeDto);
        Task<bool> DeleteEmployeeAsync(int id);

        Task<decimal> CalculateMonthlySalaryAsync(int employeeId, int actualHoursWorked);

        Task<PayrollDto> SaveMonthlyPayrollAsync(int employeeId, int actualHoursWorked, int month, int year);
    }
}
