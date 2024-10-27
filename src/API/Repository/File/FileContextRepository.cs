using Microsoft.EntityFrameworkCore;
using TestGeneratorAPI.src.API.Base;
using TestGeneratorAPI.src.API.Base.Context;
using TestGeneratorAPI.src.API.Model;
using TestGeneratorAPI.src.API.View.File;

namespace TestGeneratorAPI.src.API.Repository.File
{
    public class FileContextRepository
    {
        private readonly PrincipalDbContext _context;

        public FileContextRepository(PrincipalDbContext context)
        {
            _context = context;
        }

        public async Task<FileContext> AddAsync(FileContext entity)
        {
            _context.FileContext.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(FileContext entity)
        {
            _context.FileContext.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRangeAsync(List<FileContext> entity)
        {
            _context.FileContext.RemoveRange(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var file = await GetByIdAsync(id);
            if (file is not null)
            {
                _context.FileContext.Remove(file);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<FileContext>> GetAllAsync()
        {
            return await _context.FileContext.ToListAsync(); 
        }

        public async Task<FileContext?> GetByIdAsync(int id)
        {
            return await _context.FileContext.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<FileContext> UpdateAsync(FileContext entity)
        {
            _context.FileContext.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<FileContext>> GetFilesByFolderIdAsync(int folderId)
        {
            return await _context.FileContext.Where(f => f.FolderId == folderId).ToListAsync();
        }

        public async Task<IEnumerable<FileContext>> GetFilesByUserIdAsync(int userId)
        {
            return await _context.FileContext.Where(f => f.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<FileContext>> AddRangeAsync(List<FileContext> files)
        {
            await _context.FileContext.AddRangeAsync(files);
            await _context.SaveChangesAsync();
            return files;
        }

        public async Task<List<FileContext>> GetFilesContextByIdsArrayAndUserId(List<int> ids, int userId)
        {
            return await _context.FileContext.Where(x=> x.UserId == userId && ids.Contains(x.Id)).ToListAsync();
        }

        public async Task<FolderStructureResponse> GetUserFoldersAndFiles(int userId)
        {
          
            var root = new FolderStructureResponse
            {
                Files = await _context.FileContext
                    .Where(fc => fc.UserId == userId)
                    .Select(fc => new FileContextDto
                    {
                        FolderId = fc.FolderId,
                        FileName = fc.FileName,
                        Content = fc.Content
                    }).ToListAsync(),
                
                Folders = await _context.Folder
                    .Where(f => f.UserId == userId)
                    .Select(f => new FolderDto
                    {
                        Id = f.Id,
                        ParentFolderId = f.ParentFolderId,
                        FolderName = f.FolderName,
                    })
                    .ToListAsync()
            };

            return root;
        }
    }
}
