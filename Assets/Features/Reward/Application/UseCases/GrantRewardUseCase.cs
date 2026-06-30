using System.Collections.Generic;
using System.Linq;
using AshDefender.Features.Reward.Application.DTOs;
using AshDefender.Features.Reward.Domain.Interfaces;
using AshDefender.Features.Reward.Domain.ValueObjects;
using AshDefender.Shared.Core;
using AshDefender.Shared.Events;

namespace AshDefender.Features.Reward.Application.UseCases
{
    public class GrantRewardUseCase
    {
        private readonly IRewardService _rewardService;
        private readonly IEventBus _eventBus;

        public GrantRewardUseCase(IRewardService rewardService, IEventBus eventBus)
        {
            _rewardService = rewardService;
            _eventBus = eventBus;
        }

        public IReadOnlyList<RewardDTO> Execute(string stageId, int score)
        {
            var rewards = _rewardService.GenerateStageRewards(stageId, score);

            foreach (var reward in rewards)
            {
                _rewardService.GrantReward(reward);
                _eventBus.Publish(new RewardGrantedEvent(reward.RewardType.ToString(), reward.Amount));
            }

            return rewards.Select(r => new RewardDTO(r.RewardType, r.Amount, r.RewardType.ToString())).ToList().AsReadOnly();
        }
    }
}
