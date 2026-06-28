using System.Collections.Generic;
using AshDefender.Features.Wave.Domain.Entities;

namespace AshDefender.Features.Wave.Domain.Interfaces
{
    public interface IWaveRepository
    {
        IReadOnlyList<Wave> GetWavesForStage(string stageId);
        int GetTotalWaveCount(string stageId);
    }
}
