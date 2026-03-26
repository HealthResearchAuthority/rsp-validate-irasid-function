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

        public async Task<HarpProjectRecordDataDTO?> GetRecordByIrasIdAsync(int irasId)
        {
            _logger.LogInformation("Fetching record for IRAS ID: {IrasId}", irasId);

            var record = await _repository.GetRecordByIrasIdAsync(irasId);

            if (record is null)
                return null;

            return new HarpProjectRecordDataDTO
            {
                IRASID = record.IrasId,
                RecID = record.RecID,
                RecName = record.RecName,
                ShortProjectTitle = record.ShortStudyTitle,
                LongProjectTitle = record.FullResearchTitle
            };
        }
    }
}