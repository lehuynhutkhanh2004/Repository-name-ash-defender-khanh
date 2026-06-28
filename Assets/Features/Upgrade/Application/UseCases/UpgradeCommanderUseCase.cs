using AshDefender.Features.CommanderSkill.Domain.Interfaces;
using AshDefender.Features.Upgrade.Application.DTOs;
using AshDefender.Features.Upgrade.Application.Validators;
using AshDefender.Features.Upgrade.Domain.Interfaces;

namespace AshDefender.Features.Upgrade.Application.UseCases
{
    public class UpgradeCommanderUseCase
    {
        private readonly ICommanderSkillRepository _skillRepository;
        private readonly IUpgradeService _upgradeService;
        private readonly UpgradeValidator _validator;

        private const float PowerBonus = 5f;
        private const float CooldownReduction = 0.2f;

        public UpgradeCommanderUseCase(
            ICommanderSkillRepository skillRepository,
            IUpgradeService upgradeService,
            UpgradeValidator validator)
        {
            _skillRepository = skillRepository;
            _upgradeService = upgradeService;
            _validator = validator;
        }

        public UpgradeResultDTO Execute(UpgradeRequestDTO request)
        {
            var skill = _skillRepository.GetById(request.TargetId);
            var cost = _upgradeService.GetUpgradeCost(request.TargetId, request.Target, skill.Level);

            _validator.Validate(request, skill.Level);

            skill.Upgrade(PowerBonus, CooldownReduction);
            _skillRepository.Save(skill);

            return new UpgradeResultDTO(skill.Id, request.Target, skill.Level, cost);
        }
    }
}
