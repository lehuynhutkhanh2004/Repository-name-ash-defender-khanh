using AshDefender.Features.Enemy.Domain.Enums;

namespace AshDefender.Features.Enemy.Application.DTOs
{
    public record EnemyDTO(
        string Id,
        string Name,
        EnemyType EnemyType,
        int CurrentHealth,
        int MaxHealth,
        int Attack,
        float MoveSpeed,
        int GoldReward
    );
}
