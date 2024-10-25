using System.Runtime.CompilerServices;
using TestGeneratorAPI.src.API.Base;
using TestGeneratorAPI.src.API.Enum;
using TestGeneratorAPI.src.API.Model;
using File = TestGeneratorAPI.src.API.Model.File;

namespace TestGeneratorAPI.src.API.Interface
{
    public interface IFileService : IService<File, int>
    {
        Task<IEnumerable<File>> GetFilesByBatchProcessIdAsync(int batchProcessId);
        Task<IEnumerable<File>> GetFilesByUserIdAsync(int userId);
        Task<File> UpdateFileStatusAsync(int fileId, FileStatus status);
    }
}
