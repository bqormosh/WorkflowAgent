namespace WorkflowAgent.Contracts.Tickets;

public sealed class CreateTicketRequest
{
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string RequesterEmail { get; set; } = string.Empty;
}
