using AutoMapper;
using CompanyERP.Data;
using CompanyERP.DTOs;
using CompanyERP.Entities;
using CompanyERP.Exceptions;
using CompanyERP.IServices;
using Microsoft.EntityFrameworkCore;

namespace CompanyERP.Services
{
    public class BranchService : IBranchService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper; 

        public BranchService(AppDbContext context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<BranchDto> CreateBranchAsync(BranchDto branchDto)
        {
            var branch = _mapper.Map<Branch>(branchDto);
            await _context.SaveChangesAsync();
            return _mapper.Map<BranchDto>(branch);

        }

        public async Task<bool> DeleteBranchAsync(int id)
        {
            var branch = await _context.Branch.FirstOrDefaultAsync(b => b.Id == id);

            if (branch == null)
            {
                throw new NotFoundException($"Cannot delete. Branch with ID {id} was not found.");
            }

            _context.Branch.Remove(branch);
            await _context.SaveChangesAsync();

            return true; 
        }

        public async Task<IEnumerable<BranchDto>> GetAllBranchesAsync()
        {
            var branches = await _context.Branch.ToListAsync();

            var branchDtos = _mapper.Map<IEnumerable<BranchDto>>(branches);
            return branchDtos;
        }

        public async Task<BranchDto> GetBranchByIdAsync(int id)
        {
            var branch = await _context.Branch.FirstOrDefaultAsync(b => b.Id == id);
            if (branch == null)
            {
                throw new NotFoundException($"Branch with ID {id} was not found.");
            }
            return _mapper.Map<BranchDto>(branch);

        }

        public async Task<BranchDto> UpdateBranchAsync(BranchDto branchDto)
        {
            var existingBranch = await _context.Branch.FirstOrDefaultAsync(b => b.Id == branchDto.Id);

            if (existingBranch == null)
            {
                throw new NotFoundException($"Cannot update. Branch with ID {branchDto.Id} was not found.");
            }
             _mapper.Map(branchDto, existingBranch);

            await _context.SaveChangesAsync();
            return _mapper.Map<BranchDto>(existingBranch);
             
        }


    }
}
