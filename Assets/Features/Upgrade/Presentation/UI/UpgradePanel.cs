using AshDefender.Features.Upgrade.Application.DTOs;
using AshDefender.Features.Upgrade.Application.UseCases;
using AshDefender.Features.Upgrade.Domain.Enums;
using AshDefender.Shared.Exceptions;
using UnityEngine;
using VContainer;

namespace AshDefender.Features.Upgrade.Presentation.UI
{
    public class UpgradePanel : MonoBehaviour
    {
        private UpgradeHeroUseCase _upgradeHeroUseCase;
        private UpgradeUnitUseCase _upgradeUnitUseCase;
        private UpgradeCommanderUseCase _upgradeCommanderUseCase;
        private int _currentGold;

        [Inject]
        public void Construct(
            UpgradeHeroUseCase upgradeHeroUseCase,
            UpgradeUnitUseCase upgradeUnitUseCase,
            UpgradeCommanderUseCase upgradeCommanderUseCase)
        {
            _upgradeHeroUseCase = upgradeHeroUseCase;
            _upgradeUnitUseCase = upgradeUnitUseCase;
            _upgradeCommanderUseCase = upgradeCommanderUseCase;
        }

        public void UpdateGold(int gold) => _currentGold = gold;

        public void OnUpgradeHeroClicked(string heroId, UpgradeTarget target)
        {
            TryUpgrade(() => _upgradeHeroUseCase.Execute(new UpgradeRequestDTO(heroId, target, _currentGold)));
        }

        public void OnUpgradeUnitClicked(string unitId, UpgradeTarget target)
        {
            TryUpgrade(() => _upgradeUnitUseCase.Execute(new UpgradeRequestDTO(unitId, target, _currentGold)));
        }

        public void OnUpgradeCommanderClicked(string skillId, UpgradeTarget target)
        {
            TryUpgrade(() => _upgradeCommanderUseCase.Execute(new UpgradeRequestDTO(skillId, target, _currentGold)));
        }

        private void TryUpgrade(System.Func<UpgradeResultDTO> upgradeAction)
        {
            try
            {
                var result = upgradeAction();
                _currentGold -= result.GoldSpent;
            }
            catch (InsufficientResourcesException e)
            {
                Debug.LogWarning(e.Message);
            }
        }
    }
}
