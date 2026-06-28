using AshDefender.Features.Combat.Domain.ValueObjects;

namespace AshDefender.Features.Combat.Domain.Interfaces
{
    public interface ICombatService
    {
        DamageResult CalculateDamage(int attack, int defense, float critChance, float critMultiplier);
    }
}
