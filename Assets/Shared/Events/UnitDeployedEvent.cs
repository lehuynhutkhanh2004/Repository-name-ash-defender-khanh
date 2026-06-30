using AshDefender.Shared.Core;

namespace AshDefender.Shared.Events
{
    public record UnitDeployedEvent(string UnitId, string UnitType, float X, float Y) : IGameEvent;
}
