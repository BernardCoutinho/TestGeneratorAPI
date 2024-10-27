namespace TestGeneratorAPI.src.API.Model
{
    public class Folder
    {
        public int Id { get; set; }

        public string FolderName { get; set; }
        public int? ParentFolderId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public Folder? ParentFolder { get; set; }

        public List<FileContext> FilesContext { get; set; } = new();
    }
}
