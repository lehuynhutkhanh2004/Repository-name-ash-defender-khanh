using AshDefender.Shared.Core;

namespace AshDefender.Shared.Events
{
    public record EnemyKilledEvent(string EnemyId, int GoldReward) : IGameEvent;
}
