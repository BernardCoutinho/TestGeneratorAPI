using Microsoft.EntityFrameworkCore;
using TestGeneratorAPI.src.API.Base.Context;
using TestGeneratorAPI.src.API.Interface;
using File = TestGeneratorAPI.src.API.Model.File;

namespace TestGeneratorAPI.src.API.Repository
{
    public class FileRepository : IFileRepository
    {
        private readonly PrincipalDbContext _context;

        public FileRepository(PrincipalDbContext context)
        {
            _context = context;
        }

        public async Task<File> AddAsync(File entity)
        {
            _context.Files.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(File entity)
        {
            _context.Files.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var file = await GetByIdAsync(id);
            if (file is not null)
            {
                _context.Files.Remove(file);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<File>> GetAllAsync()
        {
            return await _context.Files.ToListAsync();
        }

        public async Task<File?> GetByIdAsync(int id)
        {
            return await _context.Files.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<File> UpdateAsync(File entity)
        {
            _context.Files.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<File>> GetFilesByBatchProcessIdAsync(int batchProcessId)
        {
            return await _context.Files.Where(f => f.BatchProcessId == batchProcessId).ToListAsync();
        }

        public async Task<IEnumerable<File>> GetFilesByUserIdAsync(int userId)
        {
            return await _context.Files.Where(f => f.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<File>> AddRangeAsync(List<File> files)
        {
            await _context.Files.AddRangeAsync(files);
            await _context.SaveChangesAsync();
            return files;
        }
    }
}
