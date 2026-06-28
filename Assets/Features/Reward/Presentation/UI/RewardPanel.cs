using System.Collections.Generic;
using AshDefender.Features.Reward.Application.DTOs;
using AshDefender.Features.Reward.Application.UseCases;
using AshDefender.Shared.Core;
using AshDefender.Shared.Events;
using UnityEngine;
using VContainer;

namespace AshDefender.Features.Reward.Presentation.UI
{
    public class RewardPanel : MonoBehaviour
    {
        private GrantRewardUseCase _grantRewardUseCase;
        private IEventBus _eventBus;
        private System.IDisposable _subscription;

        [Inject]
        public void Construct(GrantRewardUseCase grantRewardUseCase, IEventBus eventBus)
        {
            _grantRewardUseCase = grantRewardUseCase;
            _eventBus = eventBus;
        }

        private void OnEnable()
        {
            _subscription = _eventBus.Subscribe<StageCompletedEvent>(OnStageCompleted);
        }

        private void OnDisable()
        {
            _subscription?.Dispose();
        }

        private void OnStageCompleted(StageCompletedEvent e)
        {
            var rewards = _grantRewardUseCase.Execute(e.StageId, e.TotalScore);
            DisplayRewards(rewards);
        }

        private void DisplayRewards(IReadOnlyList<RewardDTO> rewards)
        {
            gameObject.SetActive(true);
        }

        public void OnContinueClicked()
        {
            gameObject.SetActive(false);
        }
    }
}
