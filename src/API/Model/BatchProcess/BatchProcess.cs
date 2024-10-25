﻿using TestGeneratorAPI.src.API.Enum;

namespace TestGeneratorAPI.src.API.Model
{
    public class BatchProcess
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public BatchStatus Status { get; set; }
        public ICollection<File> Files { get; set; }
    }
}