using System.Collections.Generic;
using System.Linq;
using AshDefender.Features.Stage.Domain.Interfaces;
using AshDefender.ScriptableObjects;
using StageEntity = AshDefender.Features.Stage.Domain.Entities.Stage;

namespace AshDefender.Features.Stage.Infrastructure.Repositories
{
    public class StageRepository : IStageRepository
    {
        private readonly Dictionary<string, StageEntity> _stages = new();

        public StageRepository(StageConfigSO[] configs)
        {
            for (var i = 0; i < configs.Length; i++)
            {
                var config = configs[i];
                var previous = config.requiredPreviousStage?.stageId;
                _stages[config.stageId] = new StageEntity(config.stageId, config.stageName, i + 1, previous);
            }
        }

        public StageEntity GetById(string stageId) =>
            _stages.TryGetValue(stageId, out var s) ? s : null;

        public IReadOnlyList<StageEntity> GetAll() =>
            _stages.Values.OrderBy(s => s.StageNumber).ToList().AsReadOnly();

        public IReadOnlyList<StageEntity> GetUnlocked() =>
            _stages.Values.Where(s => s.IsUnlocked).OrderBy(s => s.StageNumber).ToList().AsReadOnly();

        public void Save(StageEntity stage) => _stages[stage.Id] = stage;
    }
}
