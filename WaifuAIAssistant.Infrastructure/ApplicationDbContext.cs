using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WaifuAIAssistant.Domain.Entities;

namespace WaifuAIAssistant.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<ModelsCharacter> ModelsCharacter { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<CharacterEmotions> CharacterEmotions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
                //Console.WriteLine($"Using environment: {environment}");
                var config = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                    //.AddEnvironmentVariables()
                    .Build();

                string connectionString = config.GetConnectionString("DefaultConnection");
                //Console.WriteLine($"Using connection string: {connectionString}");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Users>(entity =>
            {
                entity.ToTable("Users");
                entity.HasIndex(e => e.Id).IsUnique();
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            });

            modelBuilder.Entity<CharacterEmotions>(entity =>
            {
                entity.HasIndex(e => e.Id).IsUnique();

                entity.HasOne(e => e.Character)
                    .WithMany(c => c.CharacterEmotions)
                    .HasForeignKey(e => e.CharacterId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ModelsCharacter>(entity =>
            {
                entity.ToTable("ModelCharacters");
                entity.HasIndex(e => e.Id).IsUnique();
                entity.Property(e => e.Name).IsRequired();
                entity.HasMany(c => c.CharacterEmotions)
                    .WithOne(e => e.Character)
                    .HasForeignKey(e => e.CharacterId)
                    .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.ToTable("Conversations");
                entity.HasIndex(e => e.Id).IsUnique();
                entity.HasOne(c => c.User)
                    .WithMany(u => u.Conversations)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Messages).WithOne(c => c.Conversation).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("Messages");
                entity.HasIndex(e => e.Id).IsUnique();
                entity.HasOne(m => m.Conversation)
                    .WithMany(c => c.Messages)
                    .HasForeignKey(m => m.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(m => m.Users)
                    .WithMany(u => u.Messages)
                    .HasForeignKey(m => m.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            SeedData.Initialize(modelBuilder);
        }
    }
}