using AshDefender.Features.Combat.Domain.Interfaces;
using AshDefender.Features.Combat.Domain.ValueObjects;
using UnityEngine;

namespace AshDefender.Features.Combat.Infrastructure.Services
{
    public class CombatService : ICombatService
    {
        public DamageResult CalculateDamage(int attack, int defense, float critChance, float critMultiplier)
        {
            var raw = Mathf.Max(0, attack - defense);
            var isCrit = Random.value <= critChance;
            var final = isCrit ? Mathf.RoundToInt(raw * critMultiplier) : raw;
            return new DamageResult(raw, final, isCrit);
        }
    }
}
