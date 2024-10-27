using Microsoft.EntityFrameworkCore;
using TestGeneratorAPI.src.API.Base.Context;
using TestGeneratorAPI.src.API.Model;

namespace TestGeneratorAPI.src.API.Repository.FolderRepository
{
    public class FolderRepository
    {
        private readonly PrincipalDbContext _context;

        public FolderRepository(PrincipalDbContext context)
        {
            _context = context;
        }

        public async Task<Folder> CreateFolder(Folder folder) 
        {
            _context.Folder.Add(folder);
            await _context.SaveChangesAsync();
            return folder;
        }

        public async Task<bool> DeleteFolder(Folder folder)
        {
            _context.Folder.Remove(folder);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Folder> GetFolderById(int id)
        {
           return await _context.Folder.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Folder> UpdateAsync(Folder task)
        {
            _context.Folder.Update(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<List<Folder>> GetSubfoldersByParentId(int id)
        {
            return await _context.Folder.Where(x => x.ParentFolderId == id).ToListAsync();
        }
    }
}
