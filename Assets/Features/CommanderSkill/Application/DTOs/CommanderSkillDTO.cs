using AshDefender.Features.CommanderSkill.Domain.Enums;

namespace AshDefender.Features.CommanderSkill.Application.DTOs
{
    public record CommanderSkillDTO(
        string Id,
        string Name,
        SkillType SkillType,
        int ManaCost,
        float Cooldown,
        float Power,
        int Level,
        bool IsReady
    );
}
