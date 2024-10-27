using TestGeneratorAPI.src.API.Enum;
using TestGeneratorAPI.src.API.Helper;
using TestGeneratorAPI.src.API.Interface;
using TestGeneratorAPI.src.API.Model;
using TestGeneratorAPI.src.API.Repository;
using TestGeneratorAPI.src.API.Repository.File;
using TestGeneratorAPI.src.API.Repository.FolderRepository;

namespace TestGeneratorAPI.src.API.Service
{
    public class FolderService
    {
        private readonly FolderRepository _folderRepository;
        private readonly FileContextRepository _fileContextRepository;
        private readonly FileTransactionHelper _fileTransactionHelper;


        public FolderService(FolderRepository folderRepository, FileContextRepository fileContextRepository, FileTransactionHelper fileTransactionHelper)
        {
            _folderRepository = folderRepository;
            _fileContextRepository = fileContextRepository;
            _fileTransactionHelper = fileTransactionHelper;
        }

        public async Task<Folder> CreateFolder(Folder fodler)
        {
            return await _folderRepository.CreateFolder(fodler);
        }

        public async Task<Folder> GetFolderById(int id)
        {
            return await _folderRepository.GetFolderById(id);
        }
        

        public async Task<bool> DeleteFolderById(int id)
        {   


            Folder folder = await _folderRepository.GetFolderById(id);
            return await _folderRepository.DeleteFolder(folder);
        }

        public async Task<Folder> UpdateAsync(Folder folder) => await _folderRepository.UpdateAsync(folder);

        public async Task<bool> DeleteFolderTreeById(int id)
        {
            Folder folder = await _folderRepository.GetFolderById(id);

            if (folder == null)
                return false;
           
            await DeleteFilesAndSubfolders(folder);

            return await _folderRepository.DeleteFolder(folder);
        }

        private async Task DeleteFilesAndSubfolders(Folder folder)
        {
            // Deletar todos os arquivos associados a esta pasta
            var files = await _fileContextRepository.GetFilesByFolderIdAsync(folder.Id);
            if (files.Count() > 0)
            {
                List<string> filesPaths = files.Select(x => x.Content).ToList();

                foreach (var filePath in filesPaths)
                {
                    await _fileTransactionHelper.DeleteFile(filePath);
                }

                await _fileContextRepository.DeleteRangeAsync(files.ToList());
            }

            // Obter todas as pastas filhas desta pasta
            var subfolders = await _folderRepository.GetSubfoldersByParentId(folder.Id);

            // Recursivamente deletar cada pasta filha e seus arquivos
            foreach (var subfolder in subfolders)
            {
                await DeleteFilesAndSubfolders(subfolder);
                await _folderRepository.DeleteFolder(subfolder);
            }
        }


    }
}
