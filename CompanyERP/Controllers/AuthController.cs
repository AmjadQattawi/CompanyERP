using CompanyERP.Data;
using CompanyERP.DTOs;
using CompanyERP.Entities;
using CompanyERP.Security;
using Microsoft.AspNetCore.Identity; // 💡 الـ using نظيف وموجود فوق
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompanyERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TokenGenerator _tokenGenerator;
        private readonly IPasswordHasher<Employee> _passwordHasher;

        public AuthController(AppDbContext context, TokenGenerator tokenGenerator, IPasswordHasher<Employee> passwordHasher)
        {
            _context = context;
            _tokenGenerator = tokenGenerator;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto loginRequest)
        {
            // 1️⃣ البحث ع الموظف بالإيميل
            var employee = await _context.Employee
                                         .FirstOrDefaultAsync(e => e.Email.ToLower() == loginRequest.Email.ToLower());

            if (employee == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            var result = _passwordHasher.VerifyHashedPassword(employee, employee.PasswordHash, loginRequest.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid email or password.");
            }

            var token = _tokenGenerator.GenerateToken(employee);

            var response = new AuthResponseDto
            {
                Token = token,
                FullName = employee.FullName,
                Email = employee.Email,
                Role = employee.Role.ToString()
            };

            return Ok(response);
        }
    }
}