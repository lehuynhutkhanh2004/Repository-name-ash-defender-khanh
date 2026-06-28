using AshDefender.Features.Wave.Application.DTOs;
using AshDefender.Features.Wave.Domain.Interfaces;

namespace AshDefender.Features.Wave.Application.UseCases
{
    public class StartWaveUseCase
    {
        private readonly IWaveRepository _waveRepository;

        public StartWaveUseCase(IWaveRepository waveRepository)
        {
            _waveRepository = waveRepository;
        }

        public WaveDTO Execute(string stageId, int waveIndex)
        {
            var waves = _waveRepository.GetWavesForStage(stageId);
            if (waveIndex >= waves.Count)
                throw new System.IndexOutOfRangeException($"Wave index {waveIndex} out of range for stage '{stageId}'.");

            var wave = waves[waveIndex];
            var total = _waveRepository.GetTotalWaveCount(stageId);

            var entries = new System.Collections.Generic.List<WaveEntryDTO>();
            foreach (var e in wave.Entries)
                entries.Add(new WaveEntryDTO(e.EnemyId, e.SpawnCount, e.SpawnInterval));

            return new WaveDTO(wave.WaveIndex, total, entries.AsReadOnly(), wave.IsBossWave);
        }
    }
}
