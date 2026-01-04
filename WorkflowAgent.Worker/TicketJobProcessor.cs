using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorkflowAgent.Core.AI;
using WorkflowAgent.Core.Domain;
using WorkflowAgent.Infrastructure.Persistence;

namespace WorkflowAgent.Worker;

public sealed class TicketJobProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public TicketJobProcessor(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(2));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<WorkflowAgentDbContext>();
            var triage = scope.ServiceProvider.GetRequiredService<ITriageService>();

            var now = DateTimeOffset.UtcNow;

            var job = await db.TicketJobs
                .Where(x => x.Status == JobStatus.Pending && (x.LockedUntilUtc == null || x.LockedUntilUtc < now))
                .OrderBy(x => x.CreatedAtUtc)
                .FirstOrDefaultAsync(stoppingToken);

            if (job == null)
                continue;

            job.Status = JobStatus.Processing;
            job.Attempts += 1;
            job.LockedUntilUtc = now.AddMinutes(2);

            await db.SaveChangesAsync(stoppingToken);

            try
            {
                var ticket = await db.Tickets.FirstAsync(x => x.Id == job.TicketId, stoppingToken);

                var triageResult = triage.Triage(ticket.Subject, ticket.Body, ticket.RequesterEmail);

                var extracted = new ExtractedCase
                {
                    Id = Guid.NewGuid(),
                    TicketId = ticket.Id,
                    Category = triageResult.Category,
                    Priority = triageResult.Priority,
                    EntitiesJson = triageResult.EntitiesJson,
                    Confidence = triageResult.Confidence,
                    CreatedAtUtc = DateTimeOffset.UtcNow
                };

                var run = new WorkflowRun
                {
                    Id = Guid.NewGuid(),
                    TicketId = ticket.Id,
                    ExecutionMode = ExecutionMode.Simulate,
                    Status = WorkflowRunStatus.WaitingApproval,
                    CreatedAtUtc = DateTimeOffset.UtcNow,
                    Steps = new List<WorkflowStepRun>
                    {
                        new WorkflowStepRun
                        {
                            Id = Guid.NewGuid(),
                            StepOrder = 1,
                            StepType = WorkflowStepType.DraftAcknowledgement,
                            RequiresApproval = false,
                            Status = WorkflowStepStatus.Planned,
                            InputJson = "{}",
                            OutputJson = "{}"
                        },
                        new WorkflowStepRun
                        {
                            Id = Guid.NewGuid(),
                            StepOrder = 2,
                            StepType = WorkflowStepType.CreateTicket,
                            RequiresApproval = true,
                            Status = WorkflowStepStatus.WaitingApproval,
                            InputJson = "{}",
                            OutputJson = "{}"
                        }
                    }
                };

                ticket.Status = TicketStatus.Processed;
                job.Status = JobStatus.Completed;
                job.CompletedAtUtc = DateTimeOffset.UtcNow;
                job.LockedUntilUtc = null;

                db.ExtractedCases.Add(extracted);
                db.WorkflowRuns.Add(run);

                await db.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                job.Status = JobStatus.Failed;
                job.LastError = ex.Message;
                job.LockedUntilUtc = null;
                await db.SaveChangesAsync(stoppingToken);
            }
        }
    }
}
