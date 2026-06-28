using System.Collections.Generic;
using AshDefender.Features.Stage.Application.DTOs;

namespace AshDefender.Features.Stage.Application.Interfaces
{
    public interface IStageService
    {
        StageDTO GetStage(string stageId);
        IReadOnlyList<StageDTO> GetAllStages();
        IReadOnlyList<StageDTO> GetUnlockedStages();
    }
}
