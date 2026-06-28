using System.Collections.Generic;

namespace AshDefender.Features.Wave.Domain.Entities
{
    public class WaveEntry
    {
        public string EnemyId { get; }
        public int SpawnCount { get; }
        public float SpawnInterval { get; }

        public WaveEntry(string enemyId, int spawnCount, float spawnInterval)
        {
            EnemyId = enemyId;
            SpawnCount = spawnCount;
            SpawnInterval = spawnInterval;
        }
    }

    public class Wave
    {
        public int WaveIndex { get; }
        public IReadOnlyList<WaveEntry> Entries { get; }
        public bool IsBossWave { get; }
        public string BossId { get; }

        public Wave(int waveIndex, IReadOnlyList<WaveEntry> entries, bool isBossWave = false, string bossId = null)
        {
            WaveIndex = waveIndex;
            Entries = entries;
            IsBossWave = isBossWave;
            BossId = bossId;
        }
    }
}
