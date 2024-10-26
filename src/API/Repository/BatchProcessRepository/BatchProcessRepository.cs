using Microsoft.EntityFrameworkCore;
using TestGeneratorAPI.src.API.Base.Context;
using TestGeneratorAPI.src.API.Enum;
using TestGeneratorAPI.src.API.Interface;
using TestGeneratorAPI.src.API.Model;

namespace TestGeneratorAPI.src.API.Repository.BatchRepository
{
    public class BatchProcessRepository : IBatchProcessRepository
    {
        private readonly PrincipalDbContext _context;

        public BatchProcessRepository(PrincipalDbContext context)
        {
            _context = context;
        }
        public async Task<BatchProcess> AddAsync(BatchProcess entity)
        {
            _context.BatchProcesses.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(BatchProcess entity)
        {
            _context.BatchProcesses.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var batch = await GetByIdAsync(id);
            if (batch is not null)
            {
                _context.BatchProcesses.Remove(batch);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<BatchProcess?> GetByIdAsync(int id)
        {
            return await _context.BatchProcesses.FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<BatchProcess>> GetAllAsync()
        {
            return await _context.BatchProcesses.ToListAsync();
        }

        public async Task<BatchProcess> UpdateAsync(BatchProcess entity)
        {
            _context.BatchProcesses.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<BatchProcess?> GetActiveBatchByUserIdAsync(int userId)
        {
            return await _context.BatchProcesses
                .FirstOrDefaultAsync(b => b.UserId == userId && b.Status == BatchStatus.Active);
        }
    }
}
