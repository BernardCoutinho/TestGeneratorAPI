using TestGeneratorAPI.src.API.Enum;

namespace TestGeneratorAPI.src.API.Model
{
    public class FileAnswer : FileBase
    {
        public int BatchProcessId { get; set; }
        public BatchProcess BatchProcess { get; set; }
        public string? Response { get; set; }
        public string? Question { get; internal set; }

        public string? Correction { get; internal set; }
    }
}
