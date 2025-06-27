using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
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
        //public DbSet<ModelCharacter> ModelCharacters { get; set; }
        //public DbSet<ChatLog> ChatLogs { get; set; }
        //public DbSet<CharacterEmotion> CharacterEmotions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
                Console.WriteLine($"Using environment: {environment}");
                var config = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                string connectionString = config.GetConnectionString("DefaultConnection");
                Console.WriteLine($"Using connection string: {connectionString}");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Users>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            });

            //modelBuilder.Entity<CharacterEmotion>(entity =>
            //{
            //    entity.HasKey(e => e.Id);

            //    entity.HasOne(e => e.Character)
            //        .WithMany(c => c.CharacterEmotions)
            //        .HasForeignKey(e => e.CharacterId)
            //        .OnDelete(DeleteBehavior.Cascade);
            //});

            modelBuilder.Entity<ModelsCharacter>(entity =>
            {
                entity.ToTable("ModelCharacters");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                //entity.HasMany(c => c.CharacterEmotions)
                //    .WithOne(e => e.Character)
                //    .HasForeignKey(e => e.CharacterId)
                //    .OnDelete(DeleteBehavior.Cascade);
                //entity.HasMany(c => c.ChatLogs)
                //    .WithOne(l => l.Character)
                //    .HasForeignKey(l => l.CharacterId)
                //    .OnDelete(DeleteBehavior.SetNull);
                //entity.HasMany(c => c.Users)
                //    .WithOne(u => u.PreferredCharacter)
                //    .HasForeignKey(u => u.PreferredCharacterId)
                //    .OnDelete(DeleteBehavior.SetNull);
            });

            //modelBuilder.Entity<ChatLog>(entity =>
            //{
            //    entity.HasKey(e => e.Id);
            //    entity.Property(e => e.Message).IsRequired();
            //    entity.Property(e => e.Sender).IsRequired();
            //    entity.HasOne(c => c.User)
            //        .WithMany(u => u.ChatLogs)
            //        .HasForeignKey(c => c.UserId)
            //        .OnDelete(DeleteBehavior.SetNull);
            //    entity.HasOne(c => c.Character)
            //        .WithMany(c => c.ChatLogs)
            //        .HasForeignKey(c => c.CharacterId)
            //        .OnDelete(DeleteBehavior.SetNull);
            //});
        }
    }
}