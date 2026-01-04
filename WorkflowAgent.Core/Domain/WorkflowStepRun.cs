namespace WorkflowAgent.Core.Domain;

public sealed class WorkflowStepRun
{
    public Guid Id { get; set; }
    public Guid WorkflowRunId { get; set; }
    public int StepOrder { get; set; }
    public WorkflowStepType StepType { get; set; }
    public bool RequiresApproval { get; set; }
    public WorkflowStepStatus Status { get; set; }
    public string InputJson { get; set; } = "{}";
    public string OutputJson { get; set; } = "{}";

    public WorkflowRun? WorkflowRun { get; set; }
}
