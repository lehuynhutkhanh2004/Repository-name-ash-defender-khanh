using System.Collections.Generic;
using StageEntity = AshDefender.Features.Stage.Domain.Entities.Stage;

namespace AshDefender.Features.Stage.Domain.Interfaces
{
    public interface IStageRepository
    {
        StageEntity GetById(string stageId);
        IReadOnlyList<StageEntity> GetAll();
        IReadOnlyList<StageEntity> GetUnlocked();
        void Save(StageEntity stage);
    }
}
