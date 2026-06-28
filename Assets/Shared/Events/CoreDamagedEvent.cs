using AshDefender.Shared.Core;

namespace AshDefender.Shared.Events
{
    public record CoreDamagedEvent(int Damage, int CurrentHP, int MaxHP) : IGameEvent;
}
