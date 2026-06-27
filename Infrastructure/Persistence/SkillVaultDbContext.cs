using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core database context for SkillVault.
/// Maps Domain entities to PostgreSQL tables.
/// </summary>
public class SkillVaultDbContext : DbContext
{
    public SkillVaultDbContext(DbContextOptions<SkillVaultDbContext> options) : base(options)
    {
    }

    public DbSet<Certification> Certifications => Set<Certification>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Progress> ProgressEntries => Set<Progress>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure DateTime columns to use timestamp with time zone
        // This applies to all DateTime properties across all entities
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetColumnType("timestamp with time zone");
                }
            }
        }

        // Certification configuration
        modelBuilder.Entity<Certification>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Provider).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Title).IsRequired().HasMaxLength(200);
            entity.Property(c => c.CredentialUrl).HasMaxLength(500);

            entity.HasMany(c => c.Skills)
                .WithMany(s => s.Certifications)
                .UsingEntity(j => j.ToTable("CertificationSkills"));

            entity.HasMany(c => c.ProgressEntries)
                .WithOne(p => p.Certification)
                .HasForeignKey(p => p.CertificationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Skill configuration
        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
            entity.Property(s => s.Level).IsRequired().HasMaxLength(20);
        });

        // Course configuration
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Provider).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Title).IsRequired().HasMaxLength(200);
            entity.Property(c => c.Status).IsRequired().HasMaxLength(50);
            entity.Property(c => c.Url).HasMaxLength(500);

            entity.HasMany(c => c.ProgressEntries)
                .WithOne(p => p.Course)
                .HasForeignKey(p => p.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Progress configuration
        modelBuilder.Entity<Progress>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Hours).HasColumnType("decimal(5,2)");
            entity.Property(p => p.Notes).HasMaxLength(500);

            entity.HasOne(p => p.Skill)
                .WithMany(s => s.ProgressEntries)
                .HasForeignKey(p => p.SkillId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
