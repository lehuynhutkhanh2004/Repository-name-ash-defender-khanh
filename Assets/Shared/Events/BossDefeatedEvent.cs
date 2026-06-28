using AshDefender.Shared.Core;

namespace AshDefender.Shared.Events
{
    public record BossDefeatedEvent(string BossId) : IGameEvent;
}
