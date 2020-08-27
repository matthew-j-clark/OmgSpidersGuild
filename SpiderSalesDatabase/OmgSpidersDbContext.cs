using System;

using Azure.Security.KeyVault.Secrets;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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

        public virtual DbSet<PlayerList> PlayerList { get; set; }
        public virtual DbSet<SaleRun> SaleRun { get; set; }
        public virtual DbSet<SaleRunParticipation> SaleRunParticipation { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var pw=new BasicKeyVaultClient().GetSecret("DatabaseConnectionPassword").Result;

                optionsBuilder.UseSqlServer($"Server=tcp:omgspidersdb.database.windows.net,1433;Initial Catalog=omgspiders;Persist Security Info=False;User ID=omgspiders;Password={pw};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerList>(entity =>
            {
                entity.HasKey(e => e.PlayerName);

                entity.Property(e => e.PlayerName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SaleRun>(entity =>
            {
                entity.Property(e => e.RunDate).HasColumnType("datetime");

                entity.Property(e => e.RunName).IsUnicode(false);
            });

            modelBuilder.Entity<SaleRunParticipation>(entity =>
            {
                entity.Property(e => e.Paid).HasDefaultValueSql("((0))");

                entity.Property(e => e.Player)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.PlayerNavigation)
                    .WithMany(p => p.SaleRunParticipation)
                    .HasForeignKey(d => d.Player)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SaleRunParticipation_SaleRunParticipation");

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
