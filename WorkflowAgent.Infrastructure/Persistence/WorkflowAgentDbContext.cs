using Microsoft.EntityFrameworkCore;
using WorkflowAgent.Core.Domain;

namespace WorkflowAgent.Infrastructure.Persistence;

public sealed class WorkflowAgentDbContext : DbContext
{
    public WorkflowAgentDbContext(DbContextOptions<WorkflowAgentDbContext> options) : base(options) { }

    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketJob> TicketJobs => Set<TicketJob>();
    public DbSet<ExtractedCase> ExtractedCases => Set<ExtractedCase>();
    public DbSet<WorkflowRun> WorkflowRuns => Set<WorkflowRun>();
    public DbSet<WorkflowStepRun> WorkflowStepRuns => Set<WorkflowStepRun>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Subject).HasMaxLength(300).IsRequired();
            b.Property(x => x.RequesterEmail).HasMaxLength(320).IsRequired();
            b.Property(x => x.Body).IsRequired();
        });

        modelBuilder.Entity<TicketJob>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => new { x.Status, x.CreatedAtUtc });
            b.HasOne(x => x.Ticket).WithMany().HasForeignKey(x => x.TicketId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ExtractedCase>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.TicketId);
            b.Property(x => x.EntitiesJson).IsRequired();
            b.HasOne(x => x.Ticket).WithMany().HasForeignKey(x => x.TicketId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<WorkflowRun>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.TicketId);
            b.HasOne(x => x.Ticket).WithMany().HasForeignKey(x => x.TicketId).OnDelete(DeleteBehavior.Cascade);
            b.HasMany(x => x.Steps).WithOne(x => x.WorkflowRun).HasForeignKey(x => x.WorkflowRunId);
        });

        modelBuilder.Entity<WorkflowStepRun>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => new { x.WorkflowRunId, x.StepOrder }).IsUnique();
            b.Property(x => x.InputJson).IsRequired();
            b.Property(x => x.OutputJson).IsRequired();
        });
    }
}
