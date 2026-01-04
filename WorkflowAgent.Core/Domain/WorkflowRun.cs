namespace WorkflowAgent.Core.Domain;

public sealed class WorkflowRun
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public ExecutionMode ExecutionMode { get; set; }
    public WorkflowRunStatus Status { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }

    public Ticket? Ticket { get; set; }
    public List<WorkflowStepRun> Steps { get; set; } = new();
}
