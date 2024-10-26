using TestGeneratorAPI.src.API.Enum;

namespace TestGeneratorAPI.src.API.Model
{
    public class File
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public FileStatus Status { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; }

        public int BatchProcessId { get; set; }
        public BatchProcess BatchProcess { get; set; }

        public byte[]? Content { get; set; }
    }
}
