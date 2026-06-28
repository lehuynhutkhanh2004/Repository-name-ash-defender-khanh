using AshDefender.Features.Enemy.Domain.Enums;

namespace AshDefender.Features.Enemy.Domain.Entities
{
    public class Enemy
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public EnemyType EnemyType { get; private set; }
        public int CurrentHealth { get; private set; }
        public int MaxHealth { get; private set; }
        public int Attack { get; private set; }
        public float MoveSpeed { get; private set; }
        public int GoldReward { get; private set; }

        public bool IsDead => CurrentHealth <= 0;

        public Enemy(string id, string name, EnemyType enemyType, int health, int attack, float moveSpeed, int goldReward)
        {
            Id = id;
            Name = name;
            EnemyType = enemyType;
            MaxHealth = health;
            CurrentHealth = health;
            Attack = attack;
            MoveSpeed = moveSpeed;
            GoldReward = goldReward;
        }

        public void TakeDamage(int damage)
        {
            CurrentHealth = System.Math.Max(0, CurrentHealth - damage);
        }
    }
}
