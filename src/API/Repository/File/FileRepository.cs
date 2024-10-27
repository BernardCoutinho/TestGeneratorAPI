using Microsoft.EntityFrameworkCore;
using TestGeneratorAPI.src.API.Base.Context;
using TestGeneratorAPI.src.API.Interface;
using FileAnswer = TestGeneratorAPI.src.API.Model.FileAnswer;

namespace TestGeneratorAPI.src.API.Repository
{
    public class FileRepository : IFileRepository
    {
        private readonly PrincipalDbContext _context;

        public FileRepository(PrincipalDbContext context)
        {
            _context = context;
        }

        public async Task<FileAnswer> AddAsync(FileAnswer entity)
        {
            _context.FileAnswer.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(FileAnswer entity)
        {
            _context.FileAnswer.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var file = await GetByIdAsync(id);
            if (file is not null)
            {
                _context.FileAnswer.Remove(file);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<FileAnswer>> GetAllAsync()
        {
            return await _context.FileAnswer.ToListAsync();
        }

        public async Task<FileAnswer?> GetByIdAsync(int id)
        {
            return await _context.FileAnswer.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<FileAnswer> UpdateAsync(FileAnswer entity)
        {
            _context.FileAnswer.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<FileAnswer>> GetFilesByBatchProcessIdAsync(int batchProcessId)
        {
            return await _context.FileAnswer.Where(f => f.BatchProcessId == batchProcessId).ToListAsync();
        }

        public async Task<IEnumerable<FileAnswer>> GetFilesByUserIdAsync(int userId)
        {
            return await _context.FileAnswer.Where(f => f.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<FileAnswer>> AddRangeAsync(List<FileAnswer> files)
        {
            await _context.FileAnswer.AddRangeAsync(files);
            await _context.SaveChangesAsync();
            return files;
        }
    }
}
