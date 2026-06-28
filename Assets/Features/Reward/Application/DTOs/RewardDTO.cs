using AshDefender.Features.Reward.Domain.Enums;

namespace AshDefender.Features.Reward.Application.DTOs
{
    public record RewardDTO(RewardType RewardType, int Amount, string DisplayName);
}
