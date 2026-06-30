using System.Collections.Generic;
using System.Linq;
using AshDefender.Features.Wave.Domain.Entities;
using AshDefender.Features.Wave.Domain.Interfaces;
using AshDefender.ScriptableObjects;

namespace AshDefender.Features.Wave.Infrastructure.Repositories
{
    using Wave = global::AshDefender.Features.Wave.Domain.Entities.Wave;

    public class WaveRepository : IWaveRepository
    {
        private readonly Dictionary<string, List<Wave>> _stageWaves = new();

        public WaveRepository(StageConfigSO[] stages)
        {
            foreach (var stage in stages)
            {
                var waves = new List<Wave>();
                for (var i = 0; i < stage.waves.Count; i++)
                {
                    var waveConfig = stage.waves[i];
                    var isBoss = i == stage.waves.Count - 1 && stage.bossConfig != null;
                    var entries = new List<WaveEntry>
                    {
                        new(waveConfig.enemyConfig.enemyId, waveConfig.spawnCount, waveConfig.spawnInterval)
                    };
                    waves.Add(new Wave(i, entries.AsReadOnly(), isBoss,
                        isBoss ? stage.bossConfig?.enemyId : null));
                }
                _stageWaves[stage.stageId] = waves;
            }
        }

        public IReadOnlyList<Wave> GetWavesForStage(string stageId) =>
            _stageWaves.TryGetValue(stageId, out var waves) ? waves.AsReadOnly() : new List<Wave>().AsReadOnly();

        public int GetTotalWaveCount(string stageId) =>
            _stageWaves.TryGetValue(stageId, out var waves) ? waves.Count : 0;
    }
}
