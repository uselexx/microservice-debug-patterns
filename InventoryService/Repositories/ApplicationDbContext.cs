using InventoryService.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Repositories;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options) { }

    public DbSet<MovieEntity> Movies => Set<MovieEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MovieEntity>(builder =>
        {
            builder.ToTable("movies");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.HasIndex(x => x.LegacyId);

            builder.HasIndex(x => x.ReleaseDate);

            builder.Property(x => x.LegacyId)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(x => x.VoteAverage)
                .HasPrecision(3, 1);

            builder.Property(x => x.Popularity)
                .HasPrecision(10, 4);

            builder.Property(x => x.ReleaseDate)
                .HasColumnType("date");

            builder.Property(x => x.Revenue);

            builder.Property(x => x.Budget);

            builder.Property(x => x.Homepage)
                .HasColumnType("text");

            builder.Property(x => x.ImdbId)
                .HasMaxLength(20);

            builder.Property(x => x.OriginalLanguage)
                .HasMaxLength(10);

            builder.Property(x => x.OriginalTitle)
                .HasColumnType("text");

            builder.Property(x => x.Tagline)
                .HasColumnType("text");

            builder.Property(x => x.Overview)
                .HasColumnType("text");

            builder.Property(x => x.Genres)
                .HasColumnType("text");

            builder.Property(x => x.ProductionCompanies)
                .HasColumnType("text");

            builder.Property(x => x.ProductionCountries)
                .HasColumnType("text");

            builder.Property(x => x.SpokenLanguages)
                .HasColumnType("text");

            builder.Property(x => x.Keywords)
                .HasColumnType("text");

            builder.Property(x => x.AdultsOnly)
                .HasDefaultValue(false);
        });
    }
}
