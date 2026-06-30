using AshDefender.Shared.Core;

namespace AshDefender.Shared.Events
{
    public record HeroUpgradedEvent(string HeroId, int NewLevel) : IGameEvent;
}
