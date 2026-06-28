using AshDefender.Features.Upgrade.Application.DTOs;
using AshDefender.Features.Upgrade.Domain.Interfaces;
using AshDefender.Shared.Exceptions;

namespace AshDefender.Features.Upgrade.Application.Validators
{
    public class UpgradeValidator
    {
        private readonly IUpgradeService _upgradeService;

        public UpgradeValidator(IUpgradeService upgradeService)
        {
            _upgradeService = upgradeService;
        }

        public void Validate(UpgradeRequestDTO request, int currentLevel)
        {
            var cost = _upgradeService.GetUpgradeCost(request.TargetId, request.Target, currentLevel);
            if (request.CurrentGold < cost)
                throw new InsufficientResourcesException("Gold", cost, request.CurrentGold);
        }
    }
}
