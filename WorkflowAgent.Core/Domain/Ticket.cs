
namespace WorkflowAgent.Core.Domain;

public sealed class Ticket
{
    public Guid Id { get; set; }
    public TicketSource Source { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string RequesterEmail { get; set; } = string.Empty;
    public DateTimeOffset ReceivedAtUtc { get; set; }
    public TicketStatus Status { get; set; }
}
