

namespace WorkflowAgent.Core.Domain;


public enum TicketSource
{
    Api = 1,
    Email = 2
}

public enum TicketStatus
{
    New = 1,
    Enqueued = 2,
    Processed = 3,
    Failed = 4
}

public enum JobStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4
}

public enum CaseCategory
{
    General = 1,
    AccessRequest = 2,
    BugReport = 3,
    Billing = 4
}

public enum CasePriority
{
    Low = 1,
    Medium = 2,
    High = 3
}

public enum ExecutionMode
{
    Simulate = 1,
    Execute = 2
}

public enum WorkflowRunStatus
{
    Planned = 1,
    WaitingApproval = 2,
    Running = 3,
    Completed = 4,
    Failed = 5
}

public enum WorkflowStepStatus
{
    Planned = 1,
    WaitingApproval = 2,
    Approved = 3,
    Running = 4,
    Completed = 5,
    Failed = 6
}

public enum WorkflowStepType
{
    DraftAcknowledgement = 1,
    CreateTicket = 2,
    RequestMoreInfo = 3
}
