using AshDefender.Shared.Core;

namespace AshDefender.Shared.Events
{
    public record SkillUnlockedEvent(string HeroId, string SkillId) : IGameEvent;
}
