using AshDefender.Features.Combat.Application.DTOs;
using AshDefender.Features.Combat.Application.UseCases;
using AshDefender.Shared.Constants;
using UnityEngine;
using VContainer;

namespace AshDefender.Features.Combat.Presentation.Gameplay
{
    public class CombatHandler : MonoBehaviour
    {
        private ProcessAttackUseCase _processAttackUseCase;

        [SerializeField] private string combatantId;
        [SerializeField] private int attack = 10;
        [SerializeField] private int defense = 3;
        [SerializeField] private float critChance = GameConstants.DefaultCriticalChance;
        [SerializeField] private float critMultiplier = GameConstants.CriticalHitMultiplier;

        [Inject]
        public void Construct(ProcessAttackUseCase processAttackUseCase)
        {
            _processAttackUseCase = processAttackUseCase;
        }

        public DamageResultDTO Attack(string targetId, int targetDefense)
        {
            return _processAttackUseCase.Execute(new AttackDTO(
                combatantId, targetId,
                attack, targetDefense,
                critChance, critMultiplier
            ));
        }
    }
}
