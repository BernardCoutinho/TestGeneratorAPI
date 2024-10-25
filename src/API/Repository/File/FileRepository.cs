using TestGeneratorAPI.src.API.Interface;
using File = TestGeneratorAPI.src.API.Model.File;

namespace TestGeneratorAPI.src.API.Repository
{
    public class FileRepository : IFileRepository
    {
        public Task<File> AddAsync(File entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(File entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<File>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<File?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<File>> GetFilesByBatchProcessIdAsync(int batchProcessId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<File>> GetFilesByUserIdAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<File> UpdateAsync(File entity)
        {
            throw new NotImplementedException();
        }
    }
}
