using System.Collections.Generic;
using AshDefender.Features.Enemy.Domain.Enums;

namespace AshDefender.Features.Enemy.Domain.Entities
{
    public class Boss : Enemy
    {
        public BossPhase Phase { get; private set; }
        public IReadOnlyList<string> SpecialAbilities { get; private set; }
        public bool IsEnraged => Phase == BossPhase.Enrage;

        private readonly float _phase2HealthThreshold;
        private readonly float _enrageHealthThreshold;

        public Boss(
            string id, string name, EnemyType enemyType,
            int health, int attack, float moveSpeed, int goldReward,
            IReadOnlyList<string> specialAbilities,
            float phase2Threshold = 0.6f, float enrageThreshold = 0.25f)
            : base(id, name, enemyType, health, attack, moveSpeed, goldReward)
        {
            SpecialAbilities = specialAbilities ?? new List<string>();
            Phase = BossPhase.Phase1;
            _phase2HealthThreshold = phase2Threshold;
            _enrageHealthThreshold = enrageThreshold;
        }

        public new void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            UpdatePhase();
        }

        private void UpdatePhase()
        {
            if (IsDead) return;
            var ratio = (float)CurrentHealth / MaxHealth;
            if (ratio <= _enrageHealthThreshold) Phase = BossPhase.Enrage;
            else if (ratio <= _phase2HealthThreshold) Phase = BossPhase.Phase2;
        }
    }
}
