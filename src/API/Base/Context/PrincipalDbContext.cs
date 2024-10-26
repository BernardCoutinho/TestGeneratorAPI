using TestGeneratorAPI.src.API.Model;
using Microsoft.EntityFrameworkCore;
using File = TestGeneratorAPI.src.API.Model.File;
//using TaskItem = TestGeneratorAPI.src.API.Model.TaskItem;

namespace TestGeneratorAPI.src.API.Base.Context
{
    public class PrincipalDbContext : DbContext
    {
        public PrincipalDbContext(DbContextOptions<PrincipalDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<BatchProcess> BatchProcesses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasKey(e => e.Id);
               
                entity.Property(e => e.Id)
                      .ValueGeneratedOnAdd();
                
                entity.Property(e => e.Username)
                      .IsRequired()
                      .HasMaxLength(100);
               
                entity.Property(e => e.Email)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(e => e.PasswordHash)
                      .IsRequired();

                entity.HasMany(u => u.BatchProcesses)
                    .WithOne(b => b.User)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                
                entity.HasMany(u => u.Files)
                    .WithOne(f => f.User)
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<File>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.Property(f => f.FileName)
                    .IsRequired()
                    .HasMaxLength(255);
                entity.Property(f => f.FileType)
                    .IsRequired()
                    .HasMaxLength(50);

                // Relacionamento N:1 com User
                entity.HasOne(f => f.User)
                    .WithMany(u => u.Files)
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relacionamento N:1 com BatchProcess
                entity.HasOne(f => f.BatchProcess)
                    .WithMany(b => b.Files)
                    .HasForeignKey(f => f.BatchProcessId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configurações para BatchProcess
            modelBuilder.Entity<BatchProcess>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.StartTime)
                    .IsRequired();
                entity.Property(b => b.Status)
                    .IsRequired();

                // Relacionamento N:1 com User
                entity.HasOne(b => b.User)
                    .WithMany(u => u.BatchProcesses)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
