using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestGeneratorAPI.src.API.Enum;
using TestGeneratorAPI.src.API.Interface;
using TestGeneratorAPI.src.API.Interface.Login;
using TestGeneratorAPI.src.API.Model;
using TestGeneratorAPI.src.API.Repository;
using TestGeneratorAPI.src.API.Service;
using TestGeneratorAPI.src.API.Service.File;
using TestGeneratorAPI.src.API.View.File;


namespace TestGeneratorAPI.src.API.Controller
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _service;
        private readonly FolderService _folderService;
        private readonly FileContextService _fileContextService;
        private readonly FileProcessingService _fileProcessingService;

        public FileController(IFileService service, FileProcessingService processingService, FolderService folderService, FileContextService fileContextService)
        {   
            _service = service;
            _fileProcessingService = processingService;
            _folderService = folderService;
            _fileContextService = fileContextService;
        }

        //[HttpPost("files/upload")]
        [Consumes("multipart/form-data")]
        [HttpPost("batch/create")]
        public async Task<IActionResult> CreateBatch(
            [FromForm] List<int> fileIds,
            [FromForm] List<IFormFile> images,
            [FromForm] List<string> transcriptions,
            [FromForm] string question,
            [FromForm] string recomendation,
            [FromForm] int userId)
        {
            if (fileIds == null || fileIds.Count == 0)
                return BadRequest("Nenhum arquivo enviado.");

            try
            {
                // Chama o serviço para iniciar o processamento
                await _fileProcessingService.ProcessFilesForUserAsync(
                    images, fileIds, question, recomendation, userId);
                return Ok("Batch criado e processamento iniciado.");
            }
            catch (InvalidOperationException ex)
            {
                // Se o usuário já tiver um processamento ativo, retorna um erro
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("context/folder/register")]
        public async Task<IActionResult> CreateFolder([FromBody] FolderRequestDto folderDto)
        {
            Console.WriteLine("Passou aqui");
            if(folderDto == null)
            {
                return BadRequest("Erro no arquivo.");
            }

            var folder = new Folder
            {
                UserId = folderDto.UserId,
                FolderName = folderDto.FolderName,
                ParentFolderId = folderDto.ParentFolderId
            };

            Folder folderCreated = await _folderService.CreateFolder(folder);

            return Ok(new { Folder = folderCreated });
        }

        [HttpPut("context/folder/update")]
        public async Task<IActionResult> UpdateFolder([FromBody] Folder folder)
        {
            Console.WriteLine("Passou aqui");
            if (folder == null)
            {
                return BadRequest("Erro no arquivo.");
            }

            Folder folderCreated = await _folderService.UpdateAsync(folder);

            return Ok(new { Folder = folderCreated });
        }

        [HttpDelete("context/folder/delete/{id}")]
        public async Task<IActionResult> DeleteFolderTree( int id)
        {

            bool folderCreated = await _folderService.DeleteFolderTreeById(id);

            return NoContent();
        }

        [HttpGet("/ping")]
        public string Ping()
        {
            return "true";
        }

        [HttpPost("context/file/create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateContextFile(
            [FromForm] List<IFormFile> file, 
            [FromForm] int folderId, 
            [FromForm] int userId)
        {

            FileContext fileCreated = await _fileContextService.AddAsync(file.First(), folderId, userId);

            return Ok(new { Folder = fileCreated });
        }

        [HttpDelete("context/file/delete/{id}")]
        public async Task<IActionResult> DeleteFileById(int id)
        {

            bool fileDeleted = await _fileContextService.DeleteAsync(id);

            return NoContent();
        }

        [HttpGet("context/folder/user/all/{id}")]
        public async Task<FolderStructureResponse> UpdateFolder( int id)
        {
            Console.WriteLine("Passou aqui");
            if (id == null)
            {
                return null;
            }

            FolderStructureResponse  folderCreated = await _fileProcessingService.GetUserFoldersAndFiles(id);
            return folderCreated;
          
        }
    }

}
