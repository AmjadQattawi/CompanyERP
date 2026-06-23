using CompanyERP.Data;
using CompanyERP.Entities; 
using CompanyERP.Exceptions;
using CompanyERP.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CompanyERP.BackgroundServices
{
    public class PayrollBackgroundWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PayrollBackgroundWorker> _logger;

        public PayrollBackgroundWorker(IServiceProvider serviceProvider, ILogger<PayrollBackgroundWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Payroll Background Worker started operating...");

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;

                if (now.Day == 30 && now.Hour == 0) 
                {
                    _logger.LogInformation("It's payday! Processing payrolls automatically via PayrollService...");

                    try
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                            var payrollService = scope.ServiceProvider.GetRequiredService<IEmployeeService>();

                            var allEmployees = await context.Employee.ToListAsync(stoppingToken);
                            var currentMonth = now.Month;
                            var currentYear = now.Year;

                            foreach (var employee in allEmployees)
                            {
                                try
                                {
                                    
                                    await payrollService.SaveMonthlyPayrollAsync(employee.Id, 160, currentMonth, currentYear);
                                }
                                catch (BadRequestException)
                                {
                                  
                                    _logger.LogInformation($"Payroll for employee {employee.Id} already processed manually. Skipping...");
                                }
                            }

                            _logger.LogInformation("Automated monthly payroll processing completed successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while processing automated payroll.");
                    }
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }




    }
}