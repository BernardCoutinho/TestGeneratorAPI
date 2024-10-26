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

        public ICollection<File> Files { get; set; } = new List<File>();
    }
}
