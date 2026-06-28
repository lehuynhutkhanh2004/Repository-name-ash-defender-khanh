using AshDefender.Features.Reward.Domain.Enums;

namespace AshDefender.Features.Reward.Domain.ValueObjects
{
    public record RewardItem(RewardType RewardType, int Amount);
}
