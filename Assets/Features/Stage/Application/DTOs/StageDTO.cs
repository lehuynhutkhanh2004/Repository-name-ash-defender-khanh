using AshDefender.Features.Stage.Domain.Enums;

namespace AshDefender.Features.Stage.Application.DTOs
{
    public record StageDTO(
        string Id,
        string Name,
        int StageNumber,
        StageState State,
        int BestScore,
        bool IsUnlocked,
        bool IsCompleted
    );
}
