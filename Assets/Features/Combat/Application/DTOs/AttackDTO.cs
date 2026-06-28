namespace AshDefender.Features.Combat.Application.DTOs
{
    public record AttackDTO(
        string AttackerId,
        string TargetId,
        int Attack,
        int Defense,
        float CritChance,
        float CritMultiplier
    );
}
