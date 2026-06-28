using AshDefender.Features.Stage.Application.UseCases;
using AshDefender.Shared.Core;
using AshDefender.Shared.Events;
using UnityEngine;
using VContainer;

namespace AshDefender.Features.Stage.Presentation.SceneManagement
{
    public class StageSceneController : MonoBehaviour
    {
        private CompleteStageUseCase _completeStageUseCase;
        private IEventBus _eventBus;
        private SceneLoader _sceneLoader;
        private System.IDisposable _subscription;
        private string _currentStageId;

        [Inject]
        public void Construct(CompleteStageUseCase completeStageUseCase, IEventBus eventBus, SceneLoader sceneLoader)
        {
            _completeStageUseCase = completeStageUseCase;
            _eventBus = eventBus;
            _sceneLoader = sceneLoader;
        }

        private void OnEnable()
        {
            _subscription = _eventBus.Subscribe<StageCompletedEvent>(OnStageCompleted);
        }

        private void OnDisable()
        {
            _subscription?.Dispose();
        }

        private void OnStageCompleted(StageCompletedEvent e)
        {
            _sceneLoader.LoadAsync("Victory").Forget();
        }

        public void InitializeStage(string stageId) => _currentStageId = stageId;
    }
}
