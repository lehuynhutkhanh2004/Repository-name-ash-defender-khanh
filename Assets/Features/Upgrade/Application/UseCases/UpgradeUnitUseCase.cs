using AshDefender.Features.Unit.Domain.Interfaces;
using AshDefender.Features.Upgrade.Application.DTOs;
using AshDefender.Features.Upgrade.Application.Validators;
using AshDefender.Features.Upgrade.Domain.Interfaces;

namespace AshDefender.Features.Upgrade.Application.UseCases
{
    public class UpgradeUnitUseCase
    {
        private readonly IUnitRepository _unitRepository;
        private readonly IUpgradeService _upgradeService;
        private readonly UpgradeValidator _validator;

        private const int HealthBonus = 15;
        private const int AttackBonus = 4;

        public UpgradeUnitUseCase(IUnitRepository unitRepository, IUpgradeService upgradeService, UpgradeValidator validator)
        {
            _unitRepository = unitRepository;
            _upgradeService = upgradeService;
            _validator = validator;
        }

        public UpgradeResultDTO Execute(UpgradeRequestDTO request)
        {
            var unit = _unitRepository.GetById(request.TargetId);
            var cost = _upgradeService.GetUpgradeCost(request.TargetId, request.Target, unit.Level);

            _validator.Validate(request, unit.Level);

            unit.LevelUp(HealthBonus, AttackBonus);
            _unitRepository.Save(unit);

            return new UpgradeResultDTO(unit.Id, request.Target, unit.Level, cost);
        }
    }
}
