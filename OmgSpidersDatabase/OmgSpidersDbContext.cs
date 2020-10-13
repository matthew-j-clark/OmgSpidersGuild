using System;
using System.Security.Cryptography.X509Certificates;

using Azure.Security.KeyVault.Secrets;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

using StorageUtilities;

namespace SpiderSalesDatabase
{
    public partial class OmgSpidersDbContext : DbContext
    {
        public OmgSpidersDbContext()
        {
        }

        public OmgSpidersDbContext(DbContextOptions<OmgSpidersDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<MainRegistration> MainRegistration { get; set; }
        public virtual DbSet<PlayerList> PlayerList { get; set; }
        public virtual DbSet<SaleRun> SaleRun { get; set; }
        public virtual DbSet<SaleRunParticipation> SaleRunParticipation { get; set; }
        public virtual DbSet<RoleAssignmentReaction> RoleAssignmentReaction { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var pw=new BasicKeyVaultClient().GetSecretAsync("DatabaseConnectionPassword").Result;

                optionsBuilder.UseSqlServer(
                    $"Server=tcp:omgspidersdb.database.windows.net,1433;Initial Catalog=omgspiders;" +
                    $"Persist Security Info=False;User ID=omgspiders;Password={pw};MultipleActiveResultSets=False;" +
                    $"Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
                    x => x.EnableRetryOnFailure(10, TimeSpan.FromSeconds(10), new[] { 40613, -2 }));
                
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MainRegistration>(entity =>
            {
                entity.HasIndex(e => e.DiscordMention)
                    .IsUnique();

                entity.Property(e => e.DiscordMention)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MainName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<PlayerList>(entity =>
            {
                entity.HasIndex(e => e.PlayerName)
                    .HasName("IX_PlayerList")
                    .IsUnique();

                entity.Property(e => e.DiscordMention)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FriendlyName).HasMaxLength(50);

                entity.Property(e => e.PlayerName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RoleAssignmentReaction>(entity =>
            {
                entity.HasKey(e => e.RoleAssignmentId);

                entity.HasIndex(e => new { e.MessageId, e.EmoteReference })
                    .HasName("IX_RoleAssignmentReaction");

                entity.Property(e => e.ChannelId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.EmoteReference)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.GuildId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MessageId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.RoleId)
                    .IsRequired()
                    .HasMaxLength(50);
            });
            modelBuilder.Entity<SaleRun>(entity =>
            {
                entity.Property(e => e.RunDate).HasColumnType("datetime");

                entity.Property(e => e.RunName).IsUnicode(false);
            });

            modelBuilder.Entity<SaleRunParticipation>(entity =>
            {
                entity.Property(e => e.Paid).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.SaleRunParticipation)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SaleRunParticipation_PlayerList1");

                entity.HasOne(d => d.Run)
                    .WithMany(p => p.SaleRunParticipation)
                    .HasForeignKey(d => d.RunId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SaleRunParticipation_SaleRun");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
