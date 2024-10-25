using TestGeneratorAPI.src.API.Base;
using TestGeneratorAPI.src.API.Model;

namespace TestGeneratorAPI.src.API.Interface
{
    public interface IBatchProcessRepository : IRepository<BatchProcess, int>
    {
        Task<BatchProcess> GetActiveBatchByUserIdAsync(int userId);
    }
}
