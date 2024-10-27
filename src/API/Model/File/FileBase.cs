using TestGeneratorAPI.src.API.Enum;

namespace TestGeneratorAPI.src.API.Model
{
    public class FileBase
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public FileStatus Status { get; set; }
        
        public bool IsContext { get; set; } = false;
        public int UserId { get; set; }
        public User User { get; set; }

        public string Content { get; set; }
    }
}
