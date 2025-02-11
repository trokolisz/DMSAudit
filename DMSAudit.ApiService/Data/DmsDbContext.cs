using Microsoft.EntityFrameworkCore;
using DMSAudit.ApiService.Models;

namespace DMSAudit.ApiService.Data{
    public class DmsDbContext(DbContextOptions<DmsDbContext> options) : DbContext(options){
        public DbSet<Criteria> Criterias { get; set; }
        public DbSet<CriteriaState> CriteriaStates { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<LevelState> LevelStates { get; set; }
        public DbSet<Project> Projects { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // CriteriaState to Criteria relationship
        modelBuilder.Entity<CriteriaState>()
            .HasOne(cs => cs.Criteria)
            .WithMany(c => c.CriteriaStates)
            .HasForeignKey(cs => cs.CriteriaId);

        // Level to Criteria relationship
        modelBuilder.Entity<Level>()
            .HasOne(l => l.Criteria)
            .WithMany(c => c.Levels)
            .HasForeignKey(l => l.CriteriaId);

        // LevelState to Level relationship
        modelBuilder.Entity<LevelState>()
            .HasOne(ls => ls.Level)
            .WithMany(l => l.LevelStates)
            .HasForeignKey(ls => ls.LevelId);

        // Project to Level relationship
        modelBuilder.Entity<Project>()
            .HasOne(p => p.Level)
            .WithMany(l => l.Projects)
            .HasForeignKey(p => p.LevelId);

        // Add unique constraint for Criteria Name
        modelBuilder.Entity<Criteria>()
            .HasIndex(c => c.Name)
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
    }
}