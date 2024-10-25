using TestGeneratorAPI.src.API.Interface;
using TestGeneratorAPI.src.API.Model;

namespace TestGeneratorAPI.src.API.Repository.BatchRepository
{
    public class BatchProcessRepository : IBatchProcessRepository
    {
        public Task<BatchProcess> AddAsync(BatchProcess entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(BatchProcess entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<BatchProcess> GetActiveBatchByUserIdAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BatchProcess>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<BatchProcess?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<BatchProcess> UpdateAsync(BatchProcess entity)
        {
            throw new NotImplementedException();
        }
    }
}
