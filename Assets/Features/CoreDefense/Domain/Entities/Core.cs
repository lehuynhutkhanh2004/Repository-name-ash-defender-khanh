namespace AshDefender.Features.CoreDefense.Domain.Entities
{
    public class Core
    {
        public int MaxHP { get; private set; }
        public int CurrentHP { get; private set; }

        public bool IsDestroyed => CurrentHP <= 0;
        public float HealthRatio => MaxHP > 0 ? (float)CurrentHP / MaxHP : 0f;

        public Core(int maxHP)
        {
            MaxHP = maxHP;
            CurrentHP = maxHP;
        }

        public int TakeDamage(int damage)
        {
            var actual = System.Math.Min(damage, CurrentHP);
            CurrentHP -= actual;
            return actual;
        }

        public void Repair(int amount)
        {
            CurrentHP = System.Math.Min(MaxHP, CurrentHP + amount);
        }
    }
}
