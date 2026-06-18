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

        public DbSet<User> Users { get; set; }
        public DbSet<ModelsCharacter> ModelsCharacters { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<CharacterEmotion> CharacterEmotions { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<PromptTemplate> PromptTemplates { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Id).IsUnique();
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<CharacterEmotion>(entity =>
            {
                entity.HasIndex(e => e.Id).IsUnique();

                entity.HasOne(e => e.Character)
                    .WithMany(c => c.CharacterEmotions)
                    .HasForeignKey(e => e.CharacterId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ModelsCharacter>(entity =>
            {
                entity.HasIndex(e => e.Id).IsUnique();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Backstory).IsRequired();
                entity.Property(e => e.Personality).IsRequired();
                entity.Property(e => e.AvatarUrl).IsRequired();
                entity.Property(e => e.SpeakingStyle).IsRequired();
                entity.Property(e => e.IntelligenceLevel).IsRequired();
                entity.Property(e => e.ResponseStyle).IsRequired();
                entity.Property(e => e.ExampleDialogue).IsRequired();
                entity.HasMany(c => c.CharacterEmotions)
                    .WithOne(e => e.Character)
                    .HasForeignKey(e => e.CharacterId)
                    .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.HasIndex(e => e.Id).IsUnique();
                entity.HasOne(c => c.User)
                    .WithMany(u => u.Conversations)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.ModelsCharacter)
                    .WithMany()
                    .HasForeignKey(c => c.ModelCharacterId)
                    .OnDelete(DeleteBehavior.Restrict);


                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.UpdatedAt)
                      .ValueGeneratedOnAddOrUpdate()
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.SummaryAt)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.HasMany(e => e.Messages).WithOne(c => c.Conversation).OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasIndex(e => e.Id).IsUnique();

                entity.HasOne(m => m.Conversation)
                    .WithMany(c => c.Messages)
                    .HasForeignKey(m => m.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(m => m.Users)
                    .WithMany(u => u.Messages)
                    .HasForeignKey(m => m.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.ModelsCharacter)
                    .WithMany()
                    .HasForeignKey(m => m.ModelCharacterId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PromptTemplate>(entity =>
            {
                entity.HasIndex(e => e.Id).IsUnique();
                entity.Property(e => e.PromptKey).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.Version).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();

                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.UpdatedAt)
                      .HasDefaultValueSql("GETUTCDATE()")
                      .ValueGeneratedOnAddOrUpdate();
            });

            SeedData.Initialize(modelBuilder);
        }
    }
}