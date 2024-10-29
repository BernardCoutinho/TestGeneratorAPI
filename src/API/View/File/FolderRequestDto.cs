namespace TestGeneratorAPI.src.API.View.File
{
    public class FolderRequestDto
    {
        public string FolderName { get; set; }
        public int? ParentFolderId { get; set; }
        public int UserId { get; set; }
    }
}
