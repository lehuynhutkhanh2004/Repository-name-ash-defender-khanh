using System.Collections.Generic;

namespace AshDefender.Features.Hero.Application.DTOs
{
    public record HeroDTO(
        string Id,
        string Name,
        int Level,
        int CurrentHealth,
        int MaxHealth,
        int Attack,
        int Defense,
        IReadOnlyList<string> SkillSet
    );
}
