using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkflowAgent.Contracts.Tickets;
using WorkflowAgent.Core.Domain;
using WorkflowAgent.Infrastructure.Persistence;

namespace WorkflowAgent.Api.Controllers;

[ApiController]
[Route("api/tickets")]
public sealed class TicketsController : ControllerBase
{
    private readonly WorkflowAgentDbContext _db;

    public TicketsController(WorkflowAgentDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<ActionResult<CreateTicketResponse>> Create([FromBody] CreateTicketRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Subject) || string.IsNullOrWhiteSpace(request.Body) || string.IsNullOrWhiteSpace(request.RequesterEmail))
            return BadRequest("Subject, Body, and RequesterEmail are required.");

        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            Source = TicketSource.Api,
            Subject = request.Subject.Trim(),
            Body = request.Body.Trim(),
            RequesterEmail = request.RequesterEmail.Trim(),
            ReceivedAtUtc = DateTimeOffset.UtcNow,
            Status = TicketStatus.Enqueued
        };

        var job = new TicketJob
        {
            Id = Guid.NewGuid(),
            TicketId = ticket.Id,
            Status = JobStatus.Pending,
            Attempts = 0,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        _db.Tickets.Add(ticket);
        _db.TicketJobs.Add(job);

        await _db.SaveChangesAsync(ct);

        return Ok(new CreateTicketResponse { TicketId = ticket.Id, JobId = job.Id });
    }
}
