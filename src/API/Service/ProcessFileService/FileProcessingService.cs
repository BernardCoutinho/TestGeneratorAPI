using TestGeneratorAPI.src.API.Enum;
using TestGeneratorAPI.src.API.Interface;
using TestGeneratorAPI.src.API.Model;
using System.Net.Http.Headers;
using File = TestGeneratorAPI.src.API.Model.File;

public class FileProcessingService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IFileRepository _fileRepository;
    private readonly IBatchProcessRepository _batchRepository;
    private readonly Queue<(IFormFile formFile, File fileEntry, int batchId)> _processingQueue = new Queue<(IFormFile, File, int)>();

    public FileProcessingService(
        IHttpClientFactory httpClientFactory,
        IFileRepository fileRepository,
        IBatchProcessRepository batchRepository)
    {
        _httpClientFactory = httpClientFactory;
        _fileRepository = fileRepository;
        _batchRepository = batchRepository;
    }

    // Método principal para processar arquivos para um usuário
    public async Task ProcessFilesForUserAsync(List<IFormFile> files, int userId)
    {
        // Verifica se o usuário já possui um BatchProcess ativo
        var existingBatch = await _batchRepository.GetActiveBatchByUserIdAsync(userId);
        if (existingBatch != null)
            throw new InvalidOperationException("Você já possui um batch em processamento.");

        // Cria o BatchProcess com status inicial Active
        var batch = new BatchProcess
        {
            UserId = userId,
            StartTime = DateTime.UtcNow,
            Status = BatchStatus.Active
        };
        await _batchRepository.AddAsync(batch);

        // Cria os registros de arquivos e enfileira para processamento
        var fileEntries = new List<File>();
        foreach (var file in files)
        {
            var fileEntry = new File
            {
                FileName = file.FileName,
                FileType = file.ContentType,
                Status = FileStatus.Processing,
                BatchProcessId = batch.Id
            };
            fileEntries.Add(fileEntry);

            // Adiciona à fila de processamento
            _processingQueue.Enqueue((file, fileEntry, batch.Id));
        }
       _ = await _fileRepository.AddRangeAsync(fileEntries);

        // Inicia o processamento da fila
        _ = ProcessQueueAsync();
    }

    // Processa a fila de arquivos
    private async Task ProcessQueueAsync()
    {
        while (_processingQueue.Count > 0)
        {
            var (formFile, fileEntry, batchId) = _processingQueue.Dequeue();
            await ProcessFileAsync(formFile, fileEntry, batchId);
        }
    }

    // Método para processar cada arquivo individualmente
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
            var response = await client.PostAsync("url_da_outra_api", content);

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
