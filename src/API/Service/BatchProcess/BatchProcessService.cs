using TestGeneratorAPI.src.API.Enum;
using TestGeneratorAPI.src.API.Interface;
using TestGeneratorAPI.src.API.Model;
using File = TestGeneratorAPI.src.API.Model.File;


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

        public Task<Model.BatchProcess> AddAsync(Model.BatchProcess entity)
        {
            throw new NotImplementedException();
        }

        public async Task<BatchProcess> CreateNewBatchAsync(int userId, List<IFormFile> files)
        {
            var batch = new BatchProcess
            {
                UserId = userId,
                StartTime = DateTime.UtcNow,
                Status = BatchStatus.Active,
                Files = files.Select(f => new File
                {
                    FileName = f.FileName,
                    FileType = f.ContentType,
                    Status = FileStatus.Pending
                }).ToList()
            };

            await _batchRepository.AddAsync(batch);
            return batch;
        }


        public Task<bool> DeleteAsync(Model.BatchProcess entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Model.BatchProcess>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Model.BatchProcess?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> HasActiveBatchAsync(int userId)
        => await _batchRepository.GetActiveBatchByUserIdAsync(userId) != null;

        public Task<Model.BatchProcess> UpdateAsync(Model.BatchProcess entity)
        {
            throw new NotImplementedException();
        }
    }
}
