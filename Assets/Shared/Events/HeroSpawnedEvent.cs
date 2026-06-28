using AshDefender.Shared.Core;

namespace AshDefender.Shared.Events
{
    public record HeroSpawnedEvent(string HeroId) : IGameEvent;
}
