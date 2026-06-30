using AshDefender.Features.Stage.Application.DTOs;
using AshDefender.Features.Stage.Domain.Interfaces;
using AshDefender.Shared.Core;
using AshDefender.Shared.Events;
using AshDefender.Shared.Exceptions;

namespace AshDefender.Features.Stage.Application.UseCases
{
    public class CompleteStageUseCase
    {
        private readonly IStageRepository _stageRepository;
        private readonly IEventBus _eventBus;

        public CompleteStageUseCase(IStageRepository stageRepository, IEventBus eventBus)
        {
            _stageRepository = stageRepository;
            _eventBus = eventBus;
        }

        public StageDTO Execute(string stageId, int score)
        {
            var stage = _stageRepository.GetById(stageId);
            if (stage == null)
                throw new GameException($"Stage '{stageId}' not found.");

            stage.Complete(score);
            _stageRepository.Save(stage);

            UnlockNextStage(stageId);

            _eventBus.Publish(new StageCompletedEvent(stage.Id, score));

            return new StageDTO(stage.Id, stage.Name, stage.StageNumber,
                stage.State, stage.BestScore, stage.IsUnlocked, stage.IsCompleted);
        }

        private void UnlockNextStage(string completedStageId)
        {
            var all = _stageRepository.GetAll();
            foreach (var s in all)
            {
                if (s.RequiredPreviousStageId == completedStageId && !s.IsUnlocked)
                {
                    s.Unlock();
                    _stageRepository.Save(s);
                }
            }
        }
    }
}
