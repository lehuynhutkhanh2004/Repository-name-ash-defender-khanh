using AshDefender.Shared.Core;

namespace AshDefender.Shared.Events
{
    public record EnemySpawnedEvent(string EnemyId, string EnemyType) : IGameEvent;
}
