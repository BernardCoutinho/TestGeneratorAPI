using TestGeneratorAPI.src.API.Base;
using FileAnswer = TestGeneratorAPI.src.API.Model.FileAnswer;

namespace TestGeneratorAPI.src.API.Interface
{
    public interface IFileRepository : IRepository<FileAnswer, int>
    {
        Task<IEnumerable<FileAnswer>> GetFilesByBatchProcessIdAsync(int batchProcessId);
        Task<IEnumerable<FileAnswer>> GetFilesByUserIdAsync(int userId);

        Task<IEnumerable<FileAnswer>> AddRangeAsync(List<FileAnswer> files);
    }
}
