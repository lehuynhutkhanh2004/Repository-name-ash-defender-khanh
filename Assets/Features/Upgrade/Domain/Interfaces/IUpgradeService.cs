using AshDefender.Features.Upgrade.Domain.Enums;

namespace AshDefender.Features.Upgrade.Domain.Interfaces
{
    public interface IUpgradeService
    {
        int GetUpgradeCost(string targetId, UpgradeTarget target, int currentLevel);
        bool CanUpgrade(string targetId, UpgradeTarget target, int currentGold);
    }
}
