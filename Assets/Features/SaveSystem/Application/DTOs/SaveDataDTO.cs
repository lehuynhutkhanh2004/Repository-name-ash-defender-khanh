using System.Collections.Generic;

namespace AshDefender.Features.SaveSystem.Application.DTOs
{
    public record SaveDataDTO(
        string PlayerId,
        int CurrentStageIndex,
        IReadOnlyList<string> UnlockedHeroIds,
        IReadOnlyDictionary<string, int> HeroLevels,
        IReadOnlyDictionary<string, int> UnitLevels,
        IReadOnlyList<string> CompletedStageIds,
        int Gold,
        int SoulFragments
    );
}
