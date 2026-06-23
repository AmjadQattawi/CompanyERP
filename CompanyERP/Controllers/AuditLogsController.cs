using CompanyERP.Data;
using CompanyERP.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompanyERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AuditLogsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuditLogsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditLog>>> GetAuditLogs([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var totalRecords = await _context.AuditLog.CountAsync();

            var logs = await _context.AuditLog
                .OrderByDescending(l => l.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Response.Headers.Append("X-Total-Count", totalRecords.ToString());

            return Ok(logs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuditLog>> GetAuditLogById(int id)
        {
            var log = await _context.AuditLog.FindAsync(id);

            if (log == null)
            {
                return NotFound($"Audit log with ID {id} not found.");
            }

            return Ok(log);
        }
    }
}