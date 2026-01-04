using System.Text.Json;
using WorkflowAgent.Core.AI;
using WorkflowAgent.Core.Domain;

namespace WorkflowAgent.Infrastructure.AI;

public sealed class StubTriageService : ITriageService
{
    public (CaseCategory Category, CasePriority Priority, string EntitiesJson, double Confidence) Triage(string subject, string body, string requesterEmail)
    {
        var text = (subject + " " + body).ToLowerInvariant();

        var category =
            text.Contains("access") || text.Contains("permission") || text.Contains("role") ? CaseCategory.AccessRequest :
            text.Contains("bug") || text.Contains("error") || text.Contains("exception") ? CaseCategory.BugReport :
            text.Contains("invoice") || text.Contains("billing") || text.Contains("payment") ? CaseCategory.Billing :
            CaseCategory.General;

        var priority =
            text.Contains("urgent") || text.Contains("asap") || text.Contains("down") ? CasePriority.High :
            text.Contains("soon") || text.Contains("important") ? CasePriority.Medium :
            CasePriority.Low;

        var entities = JsonSerializer.Serialize(new
        {
            requesterEmail,
            keywords = ExtractKeywords(text)
        });

        var confidence = category == CaseCategory.General ? 0.6 : 0.85;

        return (category, priority, entities, confidence);
    }

    private static string[] ExtractKeywords(string text)
    {
        var candidates = new[] { "access", "permission", "role", "bug", "error", "exception", "invoice", "billing", "payment", "urgent", "asap", "down" };
        return candidates.Where(text.Contains).Distinct().ToArray();
    }
}
