using System.Collections.Generic;
using AshDefender.Features.Enemy.Domain.Enums;

namespace AshDefender.Features.Enemy.Application.DTOs
{
    public record BossDTO(
        string Id,
        string Name,
        EnemyType EnemyType,
        int CurrentHealth,
        int MaxHealth,
        int Attack,
        float MoveSpeed,
        int GoldReward,
        BossPhase Phase,
        bool IsEnraged,
        IReadOnlyList<string> SpecialAbilities
    );
}
