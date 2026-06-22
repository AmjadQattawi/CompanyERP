using CompanyERP.DTOs;

namespace CompanyERP.IServices
{
    public interface IBranchService
    {
        Task<IEnumerable<BranchDto>> GetAllBranchesAsync();
        Task<BranchDto> GetBranchByIdAsync(int id);
        Task<BranchDto> CreateBranchAsync(BranchDto branchDto);
        Task<BranchDto> UpdateBranchAsync(BranchDto branchDto);
        Task<bool> DeleteBranchAsync(int id);

    }
}
