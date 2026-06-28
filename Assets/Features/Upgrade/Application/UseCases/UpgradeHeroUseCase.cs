using AshDefender.Features.Hero.Domain.Interfaces;
using AshDefender.Features.Upgrade.Application.DTOs;
using AshDefender.Features.Upgrade.Application.Validators;
using AshDefender.Features.Upgrade.Domain.Enums;
using AshDefender.Features.Upgrade.Domain.Interfaces;
using AshDefender.Shared.Core;
using AshDefender.Shared.Events;

namespace AshDefender.Features.Upgrade.Application.UseCases
{
    public class UpgradeHeroUseCase
    {
        private readonly IHeroRepository _heroRepository;
        private readonly IUpgradeService _upgradeService;
        private readonly UpgradeValidator _validator;
        private readonly IEventBus _eventBus;

        private const int HpBonus = 20;
        private const int AttackBonus = 5;
        private const int DefenseBonus = 3;

        public UpgradeHeroUseCase(
            IHeroRepository heroRepository,
            IUpgradeService upgradeService,
            UpgradeValidator validator,
            IEventBus eventBus)
        {
            _heroRepository = heroRepository;
            _upgradeService = upgradeService;
            _validator = validator;
            _eventBus = eventBus;
        }

        public UpgradeResultDTO Execute(UpgradeRequestDTO request)
        {
            var hero = _heroRepository.GetById(request.TargetId);
            var cost = _upgradeService.GetUpgradeCost(request.TargetId, request.Target, hero.Level);

            _validator.Validate(request, hero.Level);

            hero.LevelUp(HpBonus, AttackBonus, DefenseBonus);
            _heroRepository.Save(hero);

            _eventBus.Publish(new HeroUpgradedEvent(hero.Id, hero.Level));
            return new UpgradeResultDTO(hero.Id, request.Target, hero.Level, cost);
        }
    }
}
