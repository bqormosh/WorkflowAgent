using WorkflowAgent.Core.Domain;

namespace WorkflowAgent.Core.AI;

public interface ITriageService
{
    (CaseCategory Category, CasePriority Priority, string EntitiesJson, double Confidence) Triage(string subject, string body, string requesterEmail);
}
