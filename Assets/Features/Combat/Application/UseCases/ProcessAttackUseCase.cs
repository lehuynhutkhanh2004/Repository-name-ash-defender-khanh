using AshDefender.Features.Combat.Application.DTOs;
using AshDefender.Features.Combat.Domain.Interfaces;

namespace AshDefender.Features.Combat.Application.UseCases
{
    public class ProcessAttackUseCase
    {
        private readonly ICombatService _combatService;

        public ProcessAttackUseCase(ICombatService combatService)
        {
            _combatService = combatService;
        }

        public DamageResultDTO Execute(AttackDTO attack)
        {
            var result = _combatService.CalculateDamage(
                attack.Attack, attack.Defense,
                attack.CritChance, attack.CritMultiplier
            );

            return new DamageResultDTO(attack.AttackerId, attack.TargetId, result.FinalDamage, result.IsCritical);
        }
    }
}
