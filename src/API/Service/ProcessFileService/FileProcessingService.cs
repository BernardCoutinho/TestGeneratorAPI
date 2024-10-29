using TestGeneratorAPI.src.API.Enum;
using TestGeneratorAPI.src.API.Interface;
using TestGeneratorAPI.src.API.Model;
using System.Net.Http.Headers;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Azure.Core;
using Microsoft.VisualStudio.TestPlatform.Utilities.Helpers;
using TestGeneratorAPI.src.API.Helper;
using TestGeneratorAPI.src.API.View.File;
using TestGeneratorAPI.src.API.Repository.FolderRepository;
using TestGeneratorAPI.src.API.Repository.File;
using Azure;
using TestGeneratorAPI.src.API.Service.External;
using TestGeneratorAPI.src.API.View.External;
using Newtonsoft.Json;

public class FileProcessingService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IFileRepository _fileRepository;
    private readonly FileContextRepository _fileContextRepository;
    private readonly IBatchProcessRepository _batchRepository;
    private readonly FileTransactionHelper _fileTransactionHelper;
    private readonly Queue<FileAnswer> _processingQueue = new Queue<FileAnswer>();
    private readonly ExternalApiService _externalApiService;

    public FileProcessingService(
        IHttpClientFactory httpClientFactory,
        IFileRepository fileRepository,
        IBatchProcessRepository batchRepository,
        FileTransactionHelper fileTransactionHelper,
        FileContextRepository fileContextRepository,
        ExternalApiService externalApiService)
    {
        _httpClientFactory = httpClientFactory;
        _fileRepository = fileRepository;
        _batchRepository = batchRepository;
        _fileTransactionHelper = fileTransactionHelper;
        _fileContextRepository = fileContextRepository;
        _externalApiService = externalApiService;
    }

    // Método principal para processar arquivos para um usuário
    public async Task ProcessFilesForUserAsync(List<IFormFile> images, List<int> filesIds, string question, string recomendation, int userId)
    {
        await _externalApiService.ClearContextAsync();

        var response = await CreateContextExternalAPI(filesIds, userId);
        if (response == null)
        {
            throw new Exception("Erro");
        }

        // Verifica se o usuário já possui um BatchProcess ativo
        var existingBatch = await _batchRepository.GetActiveBatchByUserIdAsync(userId);
        //if (existingBatch != null)
        //    throw new InvalidOperationException("Você já possui um batch em processamento.");

        // Cria o BatchProcess com status inicial Active
        var batch = new BatchProcess
        {
            UserId = userId,
            StartTime = DateTime.UtcNow,
            Status = BatchStatus.Active
        };

        await _batchRepository.AddAsync(batch);

        //chamda pra AWS pra converter a imagem em texto

        // Cria os registros de arquivos e enfileira para processamento
        var fileEntries = new List<FileAnswer>();
        foreach(var image in images)
        {
            var fileEntry = new FileAnswer
            {
                UserId = userId,
                FileName = image.FileName,
                FileType = image.ContentType,
                Status = FileStatus.Processing,
                BatchProcessId = batch.Id,
                Response = "transcriptions[i],",
                Question = question,
                Content = await GenerateUrl(image),
            };
            fileEntries.Add(fileEntry);

            // Adiciona à fila de processamento
            _processingQueue.Enqueue(fileEntry);
        }
       _ = await _fileRepository.AddRangeAsync(fileEntries);


        // Inicia o processamento da fila
        _ = ProcessQueueAsync(batch.Id, recomendation);
    }

    // Processa a fila de arquivos
    private async Task ProcessQueueAsync(int batchId, string recomendation)
    {
        // chamada para iniciar contexto
        

        while (_processingQueue.Count > 0)
        {
            var fileEntry = _processingQueue.Dequeue();
            await ProcessFileAsync(fileEntry, recomendation);
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
        // chamada para finalizar contexto

    }

    // Método para processar cada arquivo individualmente
    private async Task ProcessFileAsync(FileAnswer fileEntry, string recomendation)
    {
        var client = _httpClientFactory.CreateClient();


        try
        {

            PredictRequest request= new PredictRequest
            {
                question = fileEntry.Question,
                recommendation = recomendation,
                response = fileEntry.Response
            };

            // Envia o arquivo para a API externa
            var response = await _externalApiService.PredictAsync(request);

            fileEntry.Correction = JsonConvert.SerializeObject(response);
            // Atualiza o status do arquivo no repositório
            await _fileRepository.UpdateAsync(fileEntry);
        }
        catch (Exception ex)
        {
            fileEntry.Status = FileStatus.Failed;
            await _fileRepository.UpdateAsync(fileEntry);
            Console.WriteLine($"Erro no processamento: {ex.Message}");
        }
    }

    private async Task<string> GenerateUrl(IFormFile file)
    {
        string fileUrl = "";
        // Verifica se o arquivo é uma imagem ou um PDF
        if (file.ContentType.StartsWith("image/"))
        {
            // Salva a imagem na pasta "respostas"
            fileUrl = await _fileTransactionHelper.SaveFileAsync(file, "respostas", "resposta");
        }
        else if (file.ContentType == "application/pdf")
        {
            // Salva o PDF na pasta "contextos"
            fileUrl = await _fileTransactionHelper.SaveFileAsync(file, "contextos", "contexto");
        }
        else
        {
            throw new Exception("Erro salvar arquivo");
        }


        return fileUrl;


    }

    public async Task<FolderStructureResponse> GetUserFoldersAndFiles(int id)
    {
        return await _fileContextRepository.GetUserFoldersAndFiles(id);
    }

    public async Task<List<string>> CreateContextExternalAPI(List<int> ids, int userId)
    {
        var files = await _fileContextRepository.GetFilesContextByIdsArrayAndUserId(ids, userId);
        var filesPaths = files.Select(x => x.Content).ToList();

        return await _externalApiService.UploadFilesAsync(filesPaths);
    }
}
