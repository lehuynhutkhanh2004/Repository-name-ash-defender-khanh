using System.Collections.Generic;
using AshDefender.Features.Reward.Domain.Enums;
using AshDefender.Features.Reward.Domain.Interfaces;
using AshDefender.Features.Reward.Domain.ValueObjects;
using UnityEngine;

namespace AshDefender.Features.Reward.Infrastructure.Services
{
    public class RewardService : IRewardService
    {
        private readonly Dictionary<RewardType, int> _inventory = new();

        public IReadOnlyList<RewardItem> GenerateStageRewards(string stageId, int score)
        {
            var rewards = new List<RewardItem>
            {
                new(RewardType.GoldCoin, Mathf.Max(10, score / 10)),
                new(RewardType.SoulFragment, 1)
            };

            if (score >= 100) rewards.Add(new RewardItem(RewardType.AncientScroll, 1));

            return rewards.AsReadOnly();
        }

        public void GrantReward(RewardItem reward)
        {
            _inventory.TryGetValue(reward.RewardType, out var current);
            _inventory[reward.RewardType] = current + reward.Amount;
        }

        public int GetAmount(RewardType type) =>
            _inventory.TryGetValue(type, out var amount) ? amount : 0;
    }
}
