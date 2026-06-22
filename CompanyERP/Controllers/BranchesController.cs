using CompanyERP.DTOs;
using CompanyERP.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BranchesController : ControllerBase
    {
        private readonly IBranchService _branchService;
        public BranchesController(IBranchService branchService)
        {
            _branchService = branchService;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BranchDto>>> GetBranches()
        {
            var branches = await _branchService.GetAllBranchesAsync();
            return Ok(branches);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BranchDto>> CreateBranch(BranchDto branchDto)
        {
            var createdBranch = await _branchService.CreateBranchAsync(branchDto);
            return Ok(createdBranch);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BranchDto>> GetBranchById(int id)
        {
            var branch = await _branchService.GetBranchByIdAsync(id);
            return Ok(branch);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BranchDto>> UpdateBranch(BranchDto branchDto)
        {
            var updatedBranch = await _branchService.UpdateBranchAsync(branchDto);
            return Ok(updatedBranch); 
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteBranch(int id)
        {
            await _branchService.DeleteBranchAsync(id);
            return NoContent(); 
        }


    }
}
