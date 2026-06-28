using AshDefender.Features.Upgrade.Domain.Enums;

namespace AshDefender.Features.Upgrade.Application.DTOs
{
    public record UpgradeRequestDTO(string TargetId, UpgradeTarget Target, int CurrentGold);

    public record UpgradeResultDTO(string TargetId, UpgradeTarget Target, int NewLevel, int GoldSpent);
}
