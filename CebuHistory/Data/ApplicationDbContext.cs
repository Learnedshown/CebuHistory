using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CebuHistory.Models;

namespace CebuHistory.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Story> Stories { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<HistoricalDocument> Documents { get; set; }
    public DbSet<TimelineEvent> TimelineEvents { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Bookmark> Bookmarks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Story
        modelBuilder.Entity<Story>(e =>
        {
            e.HasIndex(s => s.Slug).IsUnique();
            e.HasOne(s => s.AuthorUser)
             .WithMany(u => u.Stories)
             .HasForeignKey(s => s.AuthorId)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.SetNull);

            // ==========================================
            // NEW: User Submission Relationships
            // ==========================================

            // Relation for SubmittedByUser (who submitted the story)
            e.HasOne(s => s.SubmittedByUser)
             .WithMany()  // No navigation property back (optional)
             .HasForeignKey(s => s.SubmittedByUserId)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.SetNull);
        });

        // Comment — restrict cascades to avoid multiple-path error
        modelBuilder.Entity<Comment>(e =>
        {
            e.HasOne(c => c.Story)
             .WithMany()
             .HasForeignKey(c => c.StoryId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(c => c.User)
             .WithMany()
             .HasForeignKey(c => c.UserId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Bookmark — composite PK + restrict cascades
        modelBuilder.Entity<Bookmark>(e =>
        {
            e.HasKey(b => new { b.UserId, b.StoryId });

            e.HasOne(b => b.Story)
             .WithMany()
             .HasForeignKey(b => b.StoryId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(b => b.User)
             .WithMany()
             .HasForeignKey(b => b.UserId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Photo — restrict cascade from UploadedBy
        modelBuilder.Entity<Photo>(e =>
        {
            e.HasOne(p => p.UploadedBy)
             .WithMany()
             .HasForeignKey(p => p.UploadedById)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.SetNull);
        });

        // HistoricalDocument — restrict cascade from UploadedBy
        modelBuilder.Entity<HistoricalDocument>(e =>
        {
            e.HasOne(d => d.UploadedBy)
             .WithMany()
             .HasForeignKey(d => d.UploadedById)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}