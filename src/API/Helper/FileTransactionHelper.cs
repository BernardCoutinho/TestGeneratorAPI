namespace TestGeneratorAPI.src.API.Helper
{
    public class FileTransactionHelper
    {
        private readonly string _baseDirectory;

        public FileTransactionHelper()
        {
            // Diretório base onde os arquivos serão salvos
            _baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }

        // Método para salvar arquivos
        public async Task<string> SaveFileAsync(IFormFile file, string folderName, string baseName)
        {
            // Caminho completo do diretório onde o arquivo será salvo
            string folderPath = Path.Combine(_baseDirectory, folderName);

            // Cria o diretório se ele não existir
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Incrementar o nome com base no número de arquivos já salvos
            int fileCount = Directory.GetFiles(folderPath, $"{baseName}-*.{Path.GetExtension(file.FileName).TrimStart('.')}").Length + 1;
            string fileName = $"{baseName}-{fileCount}{Path.GetExtension(file.FileName)}";
            string filePath = Path.Combine(folderPath, fileName);

            // Salvar o arquivo no diretório
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Gera uma URL relativa para acessar o arquivo
            string fileUrl = $"/{folderName}/{fileName}";
            return fileUrl;
        }

        public async Task<bool> DeleteFile(string relativePath)
        {
            // Cria o caminho completo do arquivo com base no caminho relativo fornecido
            string filePath = Path.Combine(_baseDirectory, relativePath.TrimStart('/'));

            // Verifica se o arquivo existe e tenta deletá-lo
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    return true; // Retorna verdadeiro se o arquivo for deletado com sucesso
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao deletar o arquivo: {ex.Message}");
                    return false; // Retorna falso se houver um erro ao tentar deletar
                }
            }
            else
            {
                Console.WriteLine("Arquivo não encontrado.");
                return false; // Retorna falso se o arquivo não for encontrado
            }
        }
    }
}
