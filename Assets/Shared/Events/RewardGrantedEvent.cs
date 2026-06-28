using AshDefender.Shared.Core;

namespace AshDefender.Shared.Events
{
    public record RewardGrantedEvent(string RewardType, int Amount) : IGameEvent;
}
