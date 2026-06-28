using AshDefender.Features.Wave.Application.DTOs;

namespace AshDefender.Features.Wave.Application.Interfaces
{
    public interface IWaveService
    {
        WaveDTO GetCurrentWave(string stageId, int waveIndex);
        bool HasNextWave(string stageId, int currentWaveIndex);
    }
}
