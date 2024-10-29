using TestGeneratorAPI.src.API.Model;
using Microsoft.EntityFrameworkCore;
using FileAnswer = TestGeneratorAPI.src.API.Model.FileAnswer;
//using TaskItem = TestGeneratorAPI.src.API.Model.TaskItem;

namespace TestGeneratorAPI.src.API.Base.Context
{
    public class PrincipalDbContext : DbContext
    {
        public PrincipalDbContext(DbContextOptions<PrincipalDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<FileAnswer> FileAnswer { get; set; }
        public DbSet<Folder> Folder { get; set; }
        public DbSet<FileContext> FileContext { get; set; }
        public DbSet<BatchProcess> BatchProcesses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileContext>()
            .ToTable("FileContexts")
            .HasKey(fc => fc.Id);

            modelBuilder.Entity<FileContext>()
                .Property(fc => fc.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<FileContext>()
                .Property(fc => fc.FileName)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<FileContext>()
                .Property(fc => fc.FileType)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<FileContext>()
                .Property(fc => fc.Status)
                .IsRequired();

            modelBuilder.Entity<FileContext>()
                .Property(fc => fc.Content)
                .IsRequired();

            modelBuilder.Entity<FileContext>()
                .HasOne(fc => fc.User)
                .WithMany(u => u.FilesContext)
                .HasForeignKey(fc => fc.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<FileContext>()
                .HasOne(fc => fc.Folder)
                .WithMany(f => f.FilesContext)
                .HasForeignKey(fc => fc.FolderId)
                .OnDelete(DeleteBehavior.Cascade);

            //File Answer
            modelBuilder.Entity<FileAnswer>()
                .ToTable("FileAnswers")
                .HasKey(fa => fa.Id);

            modelBuilder.Entity<FileAnswer>()
                .Property(fa => fa.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<FileAnswer>()
                .Property(fa => fa.FileName)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<FileAnswer>()
                .Property(fa => fa.FileType)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<FileAnswer>()
                .Property(fa => fa.Status)
                .IsRequired();

            modelBuilder.Entity<FileAnswer>()
                .Property(fa => fa.Content)
                .IsRequired();

            modelBuilder.Entity<FileAnswer>()
                .Property(fa => fa.Question);

            modelBuilder.Entity<FileAnswer>()
                .Property(fa => fa.Correction)
                .HasColumnType("nvarchar(max)");

            modelBuilder.Entity<FileAnswer>()
                .HasOne(fa => fa.User)
                .WithMany(u => u.Files)
                .HasForeignKey(fa => fa.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<FileAnswer>()
                .HasOne(fa => fa.BatchProcess)
                .WithMany(bp => bp.Files)
                .HasForeignKey(fa => fa.BatchProcessId)
                .OnDelete(DeleteBehavior.Cascade);



            // Folder Configuration
            modelBuilder.Entity<Folder>()
                .HasKey(f => f.Id);

            modelBuilder.Entity<Folder>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Folder>()
                .HasOne(f => f.ParentFolder)
                .WithMany()
                .HasForeignKey(f => f.ParentFolderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Folder>()
                .Property(fa => fa.FolderName)
                .IsRequired();

            modelBuilder.Entity<Folder>()
                .HasOne(f => f.User)
                .WithMany(u => u.Folders)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // BatchProcess Configuration
            modelBuilder.Entity<BatchProcess>()
                .HasKey(bp => bp.Id);

            modelBuilder.Entity<BatchProcess>()
                .Property(bp => bp.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<BatchProcess>()
                .Property(bp => bp.Status)
                .IsRequired();

            modelBuilder.Entity<BatchProcess>()
                .HasOne(bp => bp.User)
                .WithMany(u => u.BatchProcesses)
                .HasForeignKey(bp => bp.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // User Configuration
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<User>()
                .Property(u => u.PasswordHash)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasMany(u => u.Files)
                .WithOne(fa => fa.User)
                .HasForeignKey(fa => fa.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasMany(u => u.FilesContext)
                .WithOne(fc => fc.User)
                .HasForeignKey(fc => fc.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
       
    }
}
