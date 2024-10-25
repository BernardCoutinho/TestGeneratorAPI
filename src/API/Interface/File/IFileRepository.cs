using TestGeneratorAPI.src.API.Base;
using File = TestGeneratorAPI.src.API.Model.File;

namespace TestGeneratorAPI.src.API.Interface
{
    public interface IFileRepository : IRepository<File, int>
    {
        Task<IEnumerable<File>> GetFilesByBatchProcessIdAsync(int batchProcessId);
        Task<IEnumerable<File>> GetFilesByUserIdAsync(int userId);
    }
}
