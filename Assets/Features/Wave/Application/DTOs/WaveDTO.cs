using System.Collections.Generic;

namespace AshDefender.Features.Wave.Application.DTOs
{
    public record WaveEntryDTO(string EnemyId, int SpawnCount, float SpawnInterval);

    public record WaveDTO(int WaveIndex, int TotalWaves, IReadOnlyList<WaveEntryDTO> Entries, bool IsBossWave);
}
