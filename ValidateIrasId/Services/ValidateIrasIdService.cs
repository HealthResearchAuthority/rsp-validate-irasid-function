using Microsoft.Extensions.Logging;
using ValidateIrasId.Application.Contracts.Repositories;
using ValidateIrasId.Application.Contracts.Services;
using ValidateIrasId.Application.DTO;

namespace ValidateIrasId.Services
{
    public class ValidateIrasIdService : IValidateIrasIdService
    {
        private readonly ILogger<ValidateIrasIdService> _logger;
        private readonly IHarpProjectDataRepository _repository;

        public ValidateIrasIdService(ILogger<ValidateIrasIdService> logger, IHarpProjectDataRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<HarpProjectRecord?> GetRecordByIrasIdAsync(int irasId)
        {
            _logger.LogInformation("Fetching record for IRAS ID: {IrasId}", irasId);
            return await _repository.GetRecordByIrasIdAsync(irasId);
        }
    }
}