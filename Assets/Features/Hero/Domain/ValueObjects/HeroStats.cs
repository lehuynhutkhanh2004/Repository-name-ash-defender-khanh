namespace AshDefender.Features.Hero.Domain.ValueObjects
{
    public record HeroStats(int MaxHealth, int Attack, int Defense)
    {
        public HeroStats Add(HeroStats other) =>
            new(MaxHealth + other.MaxHealth, Attack + other.Attack, Defense + other.Defense);
    }
}
