namespace AshDefender.Features.Combat.Domain.ValueObjects
{
    public record DamageResult(int RawDamage, int FinalDamage, bool IsCritical);
}
