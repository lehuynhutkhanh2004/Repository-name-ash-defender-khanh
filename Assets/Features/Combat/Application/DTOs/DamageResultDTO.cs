namespace AshDefender.Features.Combat.Application.DTOs
{
    public record DamageResultDTO(string AttackerId, string TargetId, int FinalDamage, bool IsCritical);
}
