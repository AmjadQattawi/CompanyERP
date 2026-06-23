using AutoMapper;
using CompanyERP.Data;
using CompanyERP.DTOs;
using CompanyERP.Entities;
using CompanyERP.Exceptions;
using CompanyERP.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace CompanyERP.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;
        public readonly IMapper _mapper;
        private readonly IPasswordHasher<Employee> _passwordHasher;


        public EmployeeService(AppDbContext context, IMapper mapper, IPasswordHasher<Employee> passwordHasher)
        {
            _context = context;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            var employeesFromDb = await _context.Employee
                                                .Include(e => e.Branch)
                                                .ToListAsync();
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);

            return employeesDto;
        }


        public async Task<EmployeeDto> CreateEmployeeAsync(EmployeeDto employeeDto)
        {
            var branchExists = await _context.Branch.AnyAsync(b => b.Id == employeeDto.BranchId);
            if (!branchExists)
            {
                throw new NotFoundException($"Cannot create employee. Branch with ID {employeeDto.BranchId} does not exist.");
            }

            var newEmployee = _mapper.Map<Employee>(employeeDto);

            newEmployee.PasswordHash = _passwordHasher.HashPassword(newEmployee, employeeDto.Password);

            _context.Employee.Add(newEmployee);
            await _context.SaveChangesAsync();

            return _mapper.Map<EmployeeDto>(newEmployee);
        }

        public async Task<EmployeeDto> GetEmployeeByIdAsync(int id)
        {
            var employee = await _context.Employee
                                         .Include(e => e.Branch)
                                         .FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null)
            {
                throw new NotFoundException($"Employee with ID {id} was not found.");
            }
            return _mapper.Map<EmployeeDto>(employee);
            
        }


        public async Task<EmployeeDto> UpdateEmployeeAsync(EmployeeDto employeeDto)
        {
            var existingEmployee = await _context.Employee.FirstOrDefaultAsync(e => e.Id == employeeDto.Id);

            if (existingEmployee == null)
            {
                throw new NotFoundException($"Cannot update. Employee with ID {employeeDto.Id} was not found.");
            }

            if (existingEmployee.BranchId != employeeDto.BranchId)
            {
                var branchExists = await _context.Branch.AnyAsync(b => b.Id == employeeDto.BranchId);
                if (!branchExists)
                {
                    throw new NotFoundException($"Cannot update employee. New Branch with ID {employeeDto.BranchId} does not exist.");
                }
            }

            _mapper.Map(employeeDto, existingEmployee);

            if (!string.IsNullOrEmpty(employeeDto.Password))
            {
                existingEmployee.PasswordHash = _passwordHasher.HashPassword(existingEmployee, employeeDto.Password);
            }

            await _context.SaveChangesAsync();

            return _mapper.Map<EmployeeDto>(existingEmployee);
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employee.FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                throw new NotFoundException($"Cannot delete. Employee with ID {id} was not found.");
            }

            _context.Employee.Remove(employee);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<decimal> CalculateMonthlySalaryAsync(int employeeId, int actualHoursWorked)
        {
            var employee = await _context.Employee.FirstOrDefaultAsync(e => e.Id == employeeId);
            if (employee == null)
            {
                throw new NotFoundException($"Employee with ID {employeeId} was not found.");
            }
            decimal regularHourlyRate = employee.Salary / 160;

            decimal overtimeMoney = 0;
            decimal deductionMoney = 0; 

            if (actualHoursWorked > 160)
            {
                int overtimeHours = actualHoursWorked - 160;
                overtimeMoney = overtimeHours * regularHourlyRate * 1.5m;
            }
            else if (actualHoursWorked < 160)
            {
                int missingHours = 160 - actualHoursWorked;
                deductionMoney = missingHours * regularHourlyRate;
            }
            decimal finalNetSalary = employee.Salary + overtimeMoney - deductionMoney;
            return finalNetSalary;
        }



        public async Task<PayrollDto> SaveMonthlyPayrollAsync(int employeeId, int actualHoursWorked, int month, int year)
        {
            var isPayrollAlreadyProcessed = await _context.Payroll
                .AnyAsync(p => p.EmployeeId == employeeId && p.Month == month && p.Year == year);

            if (isPayrollAlreadyProcessed)
            {
                throw new BadRequestException($"Payroll for this employee has already been processed for {month}/{year}.");
            }

            var employee = await _context.Employee.FirstOrDefaultAsync(e => e.Id == employeeId);
            if (employee == null)
            {
                throw new NotFoundException($"Employee with ID {employeeId} was not found.");
            }
            decimal regularHourlyRate = employee.Salary / 160;

            decimal overtimeMoney = 0;
            decimal deductionMoney = 0;

            if (actualHoursWorked > 160)
            {
                int overtimeHours = actualHoursWorked - 160;
                overtimeMoney = overtimeHours * regularHourlyRate * 1.5m;
            }
            else if (actualHoursWorked < 160)
            {
                int missingHours = 160 - actualHoursWorked;
                deductionMoney = missingHours * regularHourlyRate;
            }

            decimal finalNetSalary = employee.Salary + overtimeMoney - deductionMoney;

            var payroll = new Payroll
            {
                EmployeeId = employeeId,
                Month = month,
                Year = year,
                BasicSalary = employee.Salary,
                OvertimeMoney = overtimeMoney,
                DeductionMoney = deductionMoney,
                NetSalary = finalNetSalary,
                ProcessedAt = DateTime.UtcNow
            };

            _context.Payroll.Add(payroll);
            await _context.SaveChangesAsync();

            var savedPayroll = await _context.Payroll
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(p => p.Id == payroll.Id);

            return _mapper.Map<PayrollDto>(savedPayroll);
        }


    }
}
