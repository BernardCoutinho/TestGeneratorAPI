using Azure.Core;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using TestGeneratorAPI.src.API.View.External;



namespace TestGeneratorAPI.src.API.Service.External
{
    public class ExternalApiService
    {
        private readonly HttpClient _httpClient;

        private readonly string _URL = "https://b1b0-187-16-241-8.ngrok-free.app";

        public ExternalApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "string");
        }

        // Método para limpar o contexto
        public async Task<bool> ClearContextAsync()
        {
            var response = await _httpClient.DeleteAsync(_URL + "/vectors");
            return response.IsSuccessStatusCode;
        }

        // Método para fazer o upload dos arquivos
        public async Task<List<string>> UploadFilesAsync(List<string> filePaths)
        {
            string basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            using var formContent = new MultipartFormDataContent();

            foreach (var filePath in filePaths)
            {
                string novoPath = filePath.Replace("/", "\\");
                string realPath = basePath + novoPath;
                var fileStream = new FileStream(realPath, FileMode.Open, FileAccess.Read);
                var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                formContent.Add(streamContent, "files", Path.GetFileName(realPath));
            }

            var response = await _httpClient.PostAsync(_URL + "/uploadfiles", formContent);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<UploadResponse>(content);
                return result?.Filenames ?? new List<string>();
            }
            return new List<string>();
        }

        // Método para enviar uma requisição para a API /predict
  

        public async Task<PredictionResponse> PredictAsync(PredictRequest request)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(5);
                var apiUrl = _URL + "/predict";
                var json = JsonConvert.SerializeObject(request);
                Console.WriteLine($"JSON Enviado: {json}"); // Para depuração
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync(apiUrl, data);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<PredictionResponse>(content);
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Falha na requisição: {response.StatusCode}");
                        Console.WriteLine($"Erro: {errorContent}");
                        return null;
                    }
                }
                catch (TaskCanceledException ex)
                {
                    Console.WriteLine("A operação foi cancelada. Isso pode ser devido a um tempo limite excedido.");
                    Console.WriteLine($"Erro: {ex.Message}");
                    return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro: {ex.Message}");
                    return null;
                }
            }
        }
        
    }
    public class PredictionResponse
    {
        public string motive { get; set; }
        public string percentage { get; set; }
        public string ideal { get; set; }
    }

    internal class UploadResponse
    {
        public List<string> Filenames;
    }
}


