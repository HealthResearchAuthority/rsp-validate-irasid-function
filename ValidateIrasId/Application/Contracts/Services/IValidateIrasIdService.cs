using ValidateIrasId.Application.DTO;

namespace ValidateIrasId.Application.Contracts.Services
{
    public interface IValidateIrasIdService
    {
        Task<HarpProjectRecord?> GetRecordByIrasIdAsync(int irasId);
    }
}