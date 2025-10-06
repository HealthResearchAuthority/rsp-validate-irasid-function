using ValidateIrasId.Application.DTO;

namespace ValidateIrasId.Application.Contracts.Repositories
{
    public interface IHarpProjectDataRepository
    {
        Task<HarpProjectRecord?> GetRecordByIrasIdAsync(int irasId);
    }
}