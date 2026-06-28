using System.Collections.Generic;
using AshDefender.Features.Reward.Domain.ValueObjects;

namespace AshDefender.Features.Reward.Domain.Interfaces
{
    public interface IRewardService
    {
        IReadOnlyList<RewardItem> GenerateStageRewards(string stageId, int score);
        void GrantReward(RewardItem reward);
    }
}
