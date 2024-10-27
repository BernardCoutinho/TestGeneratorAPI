using System.Runtime.CompilerServices;
using TestGeneratorAPI.src.API.Base;
using TestGeneratorAPI.src.API.Enum;
using TestGeneratorAPI.src.API.Model;
using FileAnswer = TestGeneratorAPI.src.API.Model.FileAnswer;

namespace TestGeneratorAPI.src.API.Interface
{
    public interface IFileService : IService<FileAnswer, int>
    {
        Task<IEnumerable<FileAnswer>> GetFilesByBatchProcessIdAsync(int batchProcessId);
        Task<IEnumerable<FileAnswer>> GetFilesByUserIdAsync(int userId);
        Task<FileAnswer> UpdateFileStatusAsync(int fileId, FileStatus status);
    }
}
