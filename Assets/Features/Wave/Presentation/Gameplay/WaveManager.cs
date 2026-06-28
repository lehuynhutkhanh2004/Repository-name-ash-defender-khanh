using AshDefender.Features.Wave.Application.UseCases;
using AshDefender.Shared.Core;
using AshDefender.Shared.Events;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace AshDefender.Features.Wave.Presentation.Gameplay
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField] private float timeBetweenWaves = 5f;

        private StartWaveUseCase _startWaveUseCase;
        private IEventBus _eventBus;
        private string _currentStageId;
        private int _currentWaveIndex;
        private int _totalWaves;
        private int _enemiesAlive;

        [Inject]
        public void Construct(StartWaveUseCase startWaveUseCase, IEventBus eventBus)
        {
            _startWaveUseCase = startWaveUseCase;
            _eventBus = eventBus;
        }

        public void StartStage(string stageId)
        {
            _currentStageId = stageId;
            _currentWaveIndex = 0;
            RunWavesAsync().Forget();
        }

        private async UniTaskVoid RunWavesAsync()
        {
            while (true)
            {
                var waveDTO = _startWaveUseCase.Execute(_currentStageId, _currentWaveIndex);
                _totalWaves = waveDTO.TotalWaves;

                await SpawnWaveAsync(waveDTO);
                await UniTask.WaitUntil(() => _enemiesAlive <= 0);

                _currentWaveIndex++;
                if (_currentWaveIndex >= _totalWaves) break;

                await UniTask.Delay(System.TimeSpan.FromSeconds(timeBetweenWaves));
            }

            _eventBus.Publish(new StageCompletedEvent(_currentStageId, 0));
        }

        private async UniTask SpawnWaveAsync(Application.DTOs.WaveDTO wave)
        {
            foreach (var entry in wave.Entries)
            {
                for (var i = 0; i < entry.SpawnCount; i++)
                {
                    SpawnEnemy(entry.EnemyId);
                    await UniTask.Delay(System.TimeSpan.FromSeconds(entry.SpawnInterval));
                }
            }
        }

        private void SpawnEnemy(string enemyId)
        {
            if (spawnPoints.Length == 0) return;
            var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            _enemiesAlive++;
        }

        public void OnEnemyDied() => _enemiesAlive = Mathf.Max(0, _enemiesAlive - 1);
    }
}
