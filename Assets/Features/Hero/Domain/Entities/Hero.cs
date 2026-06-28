using System.Collections.Generic;

namespace AshDefender.Features.Hero.Domain.Entities
{
    public class Hero
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public int Level { get; private set; }
        public int CurrentHealth { get; private set; }
        public int MaxHealth { get; private set; }
        public int Attack { get; private set; }
        public int Defense { get; private set; }
        public IReadOnlyList<string> SkillSet { get; private set; }

        public bool IsDead => CurrentHealth <= 0;

        public Hero(string id, string name, int health, int attack, int defense, IReadOnlyList<string> skillSet)
        {
            Id = id;
            Name = name;
            Level = 1;
            MaxHealth = health;
            CurrentHealth = health;
            Attack = attack;
            Defense = defense;
            SkillSet = skillSet ?? new List<string>();
        }

        public void TakeDamage(int damage)
        {
            var effective = System.Math.Max(0, damage - Defense);
            CurrentHealth = System.Math.Max(0, CurrentHealth - effective);
        }

        public void Heal(int amount)
        {
            CurrentHealth = System.Math.Min(MaxHealth, CurrentHealth + amount);
        }

        public void LevelUp(int healthBonus, int attackBonus, int defenseBonus)
        {
            Level++;
            MaxHealth += healthBonus;
            CurrentHealth = MaxHealth;
            Attack += attackBonus;
            Defense += defenseBonus;
        }

        public void UnlockSkill(string skillId)
        {
            var list = new List<string>(SkillSet);
            if (!list.Contains(skillId))
                list.Add(skillId);
            SkillSet = list.AsReadOnly();
        }
    }
}
