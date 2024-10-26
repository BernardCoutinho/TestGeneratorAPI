using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestGeneratorAPI.src.API.Interface;


namespace TestGeneratorAPI.src.API.Controller
{
    [Authorize]
    [ApiController]
    []
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _service;
        private readonly IBatchProcessService _batchService;

        public FileController(IFileService fileService, IBatchProcessService batchService)
        {
            _service = fileService;
        }

        [HttpPost("files/upload")]
        public async Task<IActionResult> UploadFiles([FromForm] IFormFileCollection files)
        {
            var userId = /* extrair ID do usuário, talvez de um token JWT */;
            var userHasActiveBatch = await _batchService.HasActiveBatch(userId);

            if (userHasActiveBatch)
                return BadRequest("Você já possui um processamento em andamento.");

            var batch = await _batchService.CreateNewBatchAsync(userId, files);
            await _service.ProcessFilesAsync(batch);

            return Ok("Arquivos enviados para processamento.");
        }
    }
}
