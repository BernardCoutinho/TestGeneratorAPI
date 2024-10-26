using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestGeneratorAPI.src.API.Enum;
using TestGeneratorAPI.src.API.Interface;
using TestGeneratorAPI.src.API.Model;
using TestGeneratorAPI.src.API.Repository;


namespace TestGeneratorAPI.src.API.Controller
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _service;
        private readonly FileProcessingService _fileProcessingService;

        public FileController(IFileService service, FileProcessingService processingService)
        {   
            _service = service;
            _fileProcessingService = processingService;
        }

        //[HttpPost("files/upload")]
        [HttpPost("batch/create")]
        public async Task<IActionResult> CreateBatch([FromForm] List<IFormFile> files, [FromQuery] int userId)
        {
            if (files == null || files.Count == 0)
                return BadRequest("Nenhum arquivo enviado.");

            try
            {
                // Chama o serviço para iniciar o processamento
                await _fileProcessingService.ProcessFilesForUserAsync(files, userId);
                return Ok("Batch criado e processamento iniciado.");
            }
            catch (InvalidOperationException ex)
            {
                // Se o usuário já tiver um processamento ativo, retorna um erro
                return BadRequest(ex.Message);
            }
        }
    }
}
