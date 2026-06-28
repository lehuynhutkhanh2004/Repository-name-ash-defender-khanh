using AshDefender.Shared.Core;

namespace AshDefender.Shared.Events
{
    public record StageCompletedEvent(string StageId, int TotalScore) : IGameEvent;
}
