using Microsoft.EntityFrameworkCore;
using ValidateIrasId.Application.Contracts.Repositories;
using ValidateIrasId.Application.DTO;

namespace ValidateIrasId.Infrastructure.Repositories
{
    public class HarpProjectDataRepository : IHarpProjectDataRepository
    {
        private readonly HarpProjectDataDbContext _context;

        public HarpProjectDataRepository(HarpProjectDataDbContext context)
        {
            _context = context;
        }

        public async Task<HarpProjectRecord?> GetRecordByIrasIdAsync(int irasId)
        {
            return await _context.HarpProjectRecords
                .FirstOrDefaultAsync(r => r.IrasId == irasId);
        }
    }
}