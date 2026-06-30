using AshDefender.Features.Upgrade.Domain.Enums;
using AshDefender.Features.Upgrade.Domain.Interfaces;
using UnityEngine;

namespace AshDefender.Features.Upgrade.Infrastructure.Services
{
    public class UpgradeService : IUpgradeService
    {
        private const int BaseCost = 50;
        private const float CostScaling = 1.4f;

        public int GetUpgradeCost(string targetId, UpgradeTarget target, int currentLevel) =>
            Mathf.RoundToInt(BaseCost * Mathf.Pow(CostScaling, currentLevel - 1));

        public bool CanUpgrade(string targetId, UpgradeTarget target, int currentGold) =>
            currentGold >= GetUpgradeCost(targetId, target, 1);
    }
}
