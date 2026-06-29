using System.Collections.Generic;
using AshDefender.Features.Wave.Domain.Entities;

namespace AshDefender.Features.Wave.Domain.Interfaces
{
    using Wave = global::AshDefender.Features.Wave.Domain.Entities.Wave;

    public interface IWaveRepository
    {
        IReadOnlyList<Wave> GetWavesForStage(string stageId);
        int GetTotalWaveCount(string stageId);
    }
}
