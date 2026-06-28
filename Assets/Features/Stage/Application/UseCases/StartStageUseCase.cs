using AshDefender.Features.Stage.Application.DTOs;
using AshDefender.Features.Stage.Domain.Interfaces;
using AshDefender.Shared.Core;
using AshDefender.Shared.Events;
using AshDefender.Shared.Exceptions;

namespace AshDefender.Features.Stage.Application.UseCases
{
    public class StartStageUseCase
    {
        private readonly IStageRepository _stageRepository;
        private readonly IEventBus _eventBus;

        public StartStageUseCase(IStageRepository stageRepository, IEventBus eventBus)
        {
            _stageRepository = stageRepository;
            _eventBus = eventBus;
        }

        public StageDTO Execute(string stageId)
        {
            var stage = _stageRepository.GetById(stageId);
            if (stage == null)
                throw new GameException($"Stage '{stageId}' not found.");
            if (!stage.IsUnlocked)
                throw new GameException($"Stage '{stageId}' is locked.");

            _eventBus.Publish(new StageStartedEvent(stage.Id, stage.Name));

            return new StageDTO(stage.Id, stage.Name, stage.StageNumber,
                stage.State, stage.BestScore, stage.IsUnlocked, stage.IsCompleted);
        }
    }
}
