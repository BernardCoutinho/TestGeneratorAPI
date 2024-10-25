using TestGeneratorAPI.src.API.Base;
using TestGeneratorAPI.src.API.Model;

namespace TestGeneratorAPI.src.API.Interface
{
    public interface IBatchProcessService : IService<BatchProcess, int>
    {
        Task<bool> HasActiveBatchAsync(int userId);
        Task<BatchProcess> CreateNewBatchAsync(int userId, List<IFormFile> files);
    }
}
