using System.Net.Http.Headers;
using TestGeneratorAPI.src.API.Enum;
using TestGeneratorAPI.src.API.Interface;
using TestGeneratorAPI.src.API.Model;
using File = TestGeneratorAPI.src.API.Model.File;


public class FileProcessingService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IFileRepository _fileRepository;
    private readonly IBatchProcessRepository _batchRepository;
    private readonly Queue<(IFormFile formFile, File fileEntry, int batchId)> _processingQueue = new Queue<(IFormFile, File, int)>();

    private readonly static string URL_API = "";

    public FileProcessingService(
        IHttpClientFactory httpClientFactory,
        IFileRepository fileRepository,
        IBatchProcessRepository batchRepository)
    {
        _httpClientFactory = httpClientFactory;
        _fileRepository = fileRepository;
        _batchRepository = batchRepository;
    }

    // Adiciona arquivos à fila para processamento
    public void StartProcessingFilesAsync(List<IFormFile> files, List<File> fileEntries, int batchId)
    {
        for (int i = 0; i < files.Count; i++)
        {
            _processingQueue.Enqueue((files[i], fileEntries[i], batchId));
        }
    }

    // Método de execução do serviço em segundo plano
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_processingQueue.Count > 0)
            {
                var (formFile, fileEntry, batchId) = _processingQueue.Dequeue();
                await ProcessFileAsync(formFile, fileEntry, batchId);
            }
            else
            {
                await Task.Delay(1000, stoppingToken); // Aguarda um segundo antes de verificar novamente
            }
        }
    }

    // Método para processar cada arquivo
    private async Task ProcessFileAsync(IFormFile formFile, File fileEntry, int batchId)
    {
        var client = _httpClientFactory.CreateClient();

        try
        {
            // Configura o conteúdo do arquivo para envio à API externa
            var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(formFile.OpenReadStream());
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(formFile.ContentType);
            content.Add(fileContent, "file", formFile.FileName);

            // Envia o arquivo para a API externa
            var response = await client.PostAsync(URL_API, content);

            // Atualiza o status e conteúdo do arquivo conforme o retorno da API
            if (response.IsSuccessStatusCode)
            {
                fileEntry.Status = FileStatus.Processed;
                fileEntry.Content = await response.Content.ReadAsByteArrayAsync(); // Supondo que o retorno seja binário
            }
            else
            {
                fileEntry.Status = FileStatus.Failed;
            }

            // Atualiza o status do arquivo no repositório
            await _fileRepository.UpdateAsync(fileEntry);
        }
        catch (Exception ex)
        {
            fileEntry.Status = FileStatus.Failed;
            await _fileRepository.UpdateAsync(fileEntry);
            Console.WriteLine($"Erro no processamento: {ex.Message}");
        }

        // Checa se todos os arquivos no batch foram processados
        var batchFiles = await _fileRepository.GetFilesByBatchProcessIdAsync(batchId);
        if (batchFiles.All(f => f.Status == FileStatus.Processed))
        {
            var batch = await _batchRepository.GetByIdAsync(batchId);
            batch.Status = BatchStatus.Completed;
            batch.EndTime = DateTime.UtcNow;
            await _batchRepository.UpdateAsync(batch);
        }
    }
}

