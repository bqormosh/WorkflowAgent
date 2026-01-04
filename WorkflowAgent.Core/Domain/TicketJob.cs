namespace WorkflowAgent.Core.Domain;

public sealed class TicketJob
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public JobStatus Status { get; set; }
    public int Attempts { get; set; }
    public string? LastError { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset? LockedUntilUtc { get; set; }
    public DateTimeOffset? CompletedAtUtc { get; set; }

    public Ticket? Ticket { get; set; }
}
