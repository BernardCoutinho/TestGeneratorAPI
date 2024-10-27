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

public class FileProcessingService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IFileRepository _fileRepository;
    private readonly FileContextRepository _fileContextRepository;
    private readonly IBatchProcessRepository _batchRepository;
    private readonly FileTransactionHelper _fileTransactionHelper;
    private readonly Queue<FileAnswer> _processingQueue = new Queue<FileAnswer>();

    public FileProcessingService(
        IHttpClientFactory httpClientFactory,
        IFileRepository fileRepository,
        IBatchProcessRepository batchRepository,
        FileTransactionHelper fileTransactionHelper,
        FileContextRepository fileContextRepository)
    {
        _httpClientFactory = httpClientFactory;
        _fileRepository = fileRepository;
        _batchRepository = batchRepository;
        _fileTransactionHelper = fileTransactionHelper;
        _fileContextRepository = fileContextRepository;
    }

    // Método principal para processar arquivos para um usuário
    public async Task ProcessFilesForUserAsync(List<IFormFile> images, List<string> transcriptions, List<int> filesIds, string question, string recomendation, int userId)
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
        var fileEntries = new List<FileAnswer>();
        for (var i = 0; i >= 0; i++)
        {
            var fileEntry = new FileAnswer
            {
                FileName = images[i].FileName,
                FileType = images[i].ContentType,
                Status = FileStatus.Processing,
                BatchProcessId = batch.Id,
                Response = transcriptions[i],
                Question = question,
                Content = await GenerateUrl(images[i]),
            };
            fileEntries.Add(fileEntry);

            // Adiciona à fila de processamento
            _processingQueue.Enqueue(fileEntry);
        }
       _ = await _fileRepository.AddRangeAsync(fileEntries);

        // Inicia o processamento da fila
        _ = ProcessQueueAsync(filesIds, batch.Id, recomendation);
    }

    // Processa a fila de arquivos
    private async Task ProcessQueueAsync(List<int> filesIds, int batchId, string recomendation)
    {
        // chamada para iniciar contexto
        while (_processingQueue.Count > 0)
        {
            var fileEntry = _processingQueue.Dequeue();
            await ProcessFileAsync(fileEntry);
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
    private async Task ProcessFileAsync(FileAnswer fileEntry)
    {
        var client = _httpClientFactory.CreateClient();

        try
        {

            // Envia o arquivo para a API externa
            var json = JsonSerializer.Serialize(fileEntry);

            // Cria o conteúdo JSON para o corpo da requisição
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            //gravar question

            var response = await client.PostAsync("url_da_outra_api", content);

            // Atualiza o status e conteúdo do arquivo conforme o retorno da API
            if (response.IsSuccessStatusCode)
            {
                fileEntry.Status = FileStatus.Processed;

                fileEntry.Response =  response.Content.ToString();
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
}
