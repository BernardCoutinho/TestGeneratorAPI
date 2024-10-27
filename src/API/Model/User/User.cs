using TestGeneratorAPI.src.API.Model;

namespace TestGeneratorAPI.src.API.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public ICollection<BatchProcess> BatchProcesses { get; set; } = new List<BatchProcess>();

        public ICollection<FileAnswer> Files { get; set; } = new List<FileAnswer>();

        public ICollection<FileContext> FilesContext { get; set; } = new List<FileContext>();
        public ICollection<Folder> Folders { get; set; } = new List<Folder>();


    }
}
