using Azure;
using Microsoft.VisualBasic.FileIO;
using TestGeneratorAPI.src.API.Enum;
using TestGeneratorAPI.src.API.Helper;
using TestGeneratorAPI.src.API.Interface;
using TestGeneratorAPI.src.API.Model;
using TestGeneratorAPI.src.API.Repository;
using TestGeneratorAPI.src.API.Repository.File;

namespace TestGeneratorAPI.src.API.Service.File
{
    public class FileContextService
    {
        private readonly FileContextRepository _fileContextRepository;
        private readonly FileTransactionHelper _fileTransactionHelper;

        public FileContextService(FileContextRepository fileContextRepository, FileTransactionHelper fileTransactionHelper)
        {
            _fileContextRepository = fileContextRepository;
            _fileTransactionHelper = fileTransactionHelper;
        }

        public async Task<FileContext> AddAsync(IFormFile file, int folderId, int userId) 
        {
            Console.WriteLine("");
            FileContext context = new FileContext
            {
                UserId = userId,
                FolderId = folderId,
                FileName = file.FileName,
                FileType = file.ContentType,
                Content = await _fileTransactionHelper.SaveFileAsync(file, "contextos", "contexto")
            };
            
            return await _fileContextRepository.AddAsync(context);

        }

        public async Task<bool> DeleteAsync(int id)
        {
            FileContext context = await _fileContextRepository.GetByIdAsync(id);

            return await _fileTransactionHelper.DeleteFile(context.Content);
        }

    }
}
