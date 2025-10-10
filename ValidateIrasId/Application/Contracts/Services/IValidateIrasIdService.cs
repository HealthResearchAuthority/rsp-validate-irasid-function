using ValidateIrasId.Application.DTO;

namespace ValidateIrasId.Application.Contracts.Services
{
    public interface IValidateIrasIdService
    {
        Task<HarpProjectRecordDataDTO?> GetRecordByIrasIdAsync(int irasId);
    }
}