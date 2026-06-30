namespace AshDefender.Features.CoreDefense.Application.DTOs
{
    public record CoreDTO(int CurrentHP, int MaxHP, float HealthRatio, bool IsDestroyed);
}
