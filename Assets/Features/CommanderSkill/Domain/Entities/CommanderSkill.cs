using AshDefender.Features.CommanderSkill.Domain.Enums;

namespace AshDefender.Features.CommanderSkill.Domain.Entities
{
    public class CommanderSkill
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public SkillType SkillType { get; private set; }
        public int ManaCost { get; private set; }
        public float Cooldown { get; private set; }
        public float Power { get; private set; }
        public int Level { get; private set; }

        private float _cooldownTimer;

        public bool IsReady => _cooldownTimer <= 0f;

        public CommanderSkill(string id, string name, SkillType skillType, int manaCost, float cooldown, float power)
        {
            Id = id;
            Name = name;
            SkillType = skillType;
            ManaCost = manaCost;
            Cooldown = cooldown;
            Power = power;
            Level = 1;
        }

        public void Activate()
        {
            _cooldownTimer = Cooldown;
        }

        public void TickCooldown(float deltaTime)
        {
            if (_cooldownTimer > 0f)
                _cooldownTimer -= deltaTime;
        }

        public void Upgrade(float powerBonus, float cooldownReduction)
        {
            Level++;
            Power += powerBonus;
            Cooldown = System.Math.Max(0.5f, Cooldown - cooldownReduction);
        }
    }
}
