using Microsoft.AspNetCore.Http.HttpResults;
using TestGeneratorAPI.src.API.Base;
using TestGeneratorAPI.src.API.Enum;
using TestGeneratorAPI.src.API.Interface;
using TestGeneratorAPI.src.API.Model;
using FileAnswer = TestGeneratorAPI.src.API.Model.FileAnswer;

namespace TestGeneratorAPI.src.API.Service
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;

        public FileService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public async Task<FileAnswer> GetByIdAsync(int id) => await _fileRepository.GetByIdAsync(id);

        public async Task<IEnumerable<FileAnswer>> GetAllAsync() => await _fileRepository.GetAllAsync();

        public async Task<IEnumerable<FileAnswer>> GetFilesByBatchProcessIdAsync(int batchProcessId)
            => await _fileRepository.GetFilesByBatchProcessIdAsync(batchProcessId);

        public async Task<IEnumerable<FileAnswer>> GetFilesByUserIdAsync(int userId)
            => await _fileRepository.GetFilesByUserIdAsync(userId);

        public async Task<FileAnswer> AddAsync(FileAnswer file) => await _fileRepository.AddAsync(file);

        public async Task<FileAnswer> UpdateFileStatusAsync(int fileId, FileStatus status)
        {
            var file = await _fileRepository.GetByIdAsync(fileId);
            if (file == null)
            {
                throw new Exception("");
            }

            file.Status = status;
            return await _fileRepository.UpdateAsync(file);
        }

        public async Task<FileAnswer> UpdateAsync(FileAnswer file) => await _fileRepository.UpdateAsync(file);

        public async Task<bool> DeleteAsync(FileAnswer file) => await _fileRepository.DeleteAsync(file);
        public async Task<bool> DeleteByIdAsync(int id) => await _fileRepository.DeleteByIdAsync(id);
    }
}
