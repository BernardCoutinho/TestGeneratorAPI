using Microsoft.AspNetCore.Http.HttpResults;
using TestGeneratorAPI.src.API.Base;
using TestGeneratorAPI.src.API.Enum;
using TestGeneratorAPI.src.API.Interface;
using TestGeneratorAPI.src.API.Model;
using File = TestGeneratorAPI.src.API.Model.File;

namespace TestGeneratorAPI.src.API.Service
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;

        public FileService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public async Task<File> GetByIdAsync(int id) => await _fileRepository.GetByIdAsync(id);

        public async Task<IEnumerable<File>> GetAllAsync() => await _fileRepository.GetAllAsync();

        public async Task<IEnumerable<File>> GetFilesByBatchProcessIdAsync(int batchProcessId)
            => await _fileRepository.GetFilesByBatchProcessIdAsync(batchProcessId);

        public async Task<IEnumerable<File>> GetFilesByUserIdAsync(int userId)
            => await _fileRepository.GetFilesByUserIdAsync(userId);

        public async Task<File> AddAsync(File file) => await _fileRepository.AddAsync(file);

        public async Task<File> UpdateFileStatusAsync(int fileId, FileStatus status)
        {
            var file = await _fileRepository.GetByIdAsync(fileId);
            if (file == null)
            {
                throw new Exception("");
            }

            file.Status = status;
            return await _fileRepository.UpdateAsync(file);
        }

        public async Task<File> UpdateAsync(File file) => await _fileRepository.UpdateAsync(file);

        public async Task<bool> DeleteAsync(File file) => await _fileRepository.DeleteAsync(file);
        public async Task<bool> DeleteByIdAsync(int id) => await _fileRepository.DeleteByIdAsync(id);
    }
}
