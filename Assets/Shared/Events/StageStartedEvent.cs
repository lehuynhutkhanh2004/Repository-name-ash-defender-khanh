using AshDefender.Shared.Core;

namespace AshDefender.Shared.Events
{
    public record StageStartedEvent(string StageId, string StageName) : IGameEvent;
}
