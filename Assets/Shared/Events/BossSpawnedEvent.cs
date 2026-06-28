using AshDefender.Shared.Core;

namespace AshDefender.Shared.Events
{
    public record BossSpawnedEvent(string BossId, string BossName) : IGameEvent;
}
