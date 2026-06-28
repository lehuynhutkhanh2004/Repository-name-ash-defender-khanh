using AshDefender.Features.Unit.Domain.Enums;

namespace AshDefender.Features.Unit.Domain.Entities
{
    public class Unit
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public UnitType UnitType { get; private set; }
        public int Cost { get; private set; }
        public int CurrentHealth { get; private set; }
        public int MaxHealth { get; private set; }
        public int Attack { get; private set; }
        public float MoveSpeed { get; private set; }
        public int Level { get; private set; }

        public bool IsDead => CurrentHealth <= 0;

        public Unit(string id, string name, UnitType unitType, int cost, int health, int attack, float moveSpeed)
        {
            Id = id;
            Name = name;
            UnitType = unitType;
            Cost = cost;
            MaxHealth = health;
            CurrentHealth = health;
            Attack = attack;
            MoveSpeed = moveSpeed;
            Level = 1;
        }

        public void TakeDamage(int damage)
        {
            CurrentHealth = System.Math.Max(0, CurrentHealth - damage);
        }

        public void LevelUp(int healthBonus, int attackBonus)
        {
            Level++;
            MaxHealth += healthBonus;
            CurrentHealth = MaxHealth;
            Attack += attackBonus;
        }
    }
}
