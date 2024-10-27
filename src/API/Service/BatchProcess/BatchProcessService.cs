using TestGeneratorAPI.src.API.Enum;
using TestGeneratorAPI.src.API.Interface;
using TestGeneratorAPI.src.API.Model;
using File = TestGeneratorAPI.src.API.Model.FileAnswer;


namespace TestGeneratorAPI.src.API.Service
{
    public class BatchProcessService : IBatchProcessService
    {
        private readonly IBatchProcessRepository _batchRepository;
        private readonly IFileRepository _fileRepository;

        public BatchProcessService(IBatchProcessRepository batchRepository, IFileRepository fileRepository)
        {
            _batchRepository = batchRepository;
            _fileRepository = fileRepository;
        }

        public async Task<BatchProcess> CreateNewBatchAsync(int userId, List<IFormFile> files)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> HasActiveBatchAsync(int userId)
        => await _batchRepository.GetActiveBatchByUserIdAsync(userId) != null;

        public async Task<BatchProcess> GetByIdAsync(int id) => await _batchRepository.GetByIdAsync(id);

        public async Task<IEnumerable<BatchProcess>> GetAllAsync() => await _batchRepository.GetAllAsync();

        public Task<BatchProcess> UpdateAsync(BatchProcess batch) => _batchRepository.UpdateAsync(batch);

        public Task<bool> DeleteAsync(BatchProcess batch) => _batchRepository.DeleteAsync(batch);

        public Task<BatchProcess> AddAsync(BatchProcess entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
