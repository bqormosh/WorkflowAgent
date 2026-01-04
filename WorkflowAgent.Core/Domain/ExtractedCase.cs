namespace WorkflowAgent.Core.Domain;

public sealed class ExtractedCase
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public CaseCategory Category { get; set; }
    public CasePriority Priority { get; set; }
    public string EntitiesJson { get; set; } = "{}";
    public double Confidence { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }

    public Ticket? Ticket { get; set; }
}
