using AshDefender.Features.Unit.Domain.Enums;

namespace AshDefender.Features.Unit.Application.DTOs
{
    public record UnitDTO(
        string Id,
        string Name,
        UnitType UnitType,
        int Cost,
        int CurrentHealth,
        int MaxHealth,
        int Attack,
        float MoveSpeed,
        int Level
    );
}
