namespace WorkflowAgent.Contracts.Tickets;

public sealed class CreateTicketResponse
{
    public Guid TicketId { get; set; }
    public Guid JobId { get; set; }
}
