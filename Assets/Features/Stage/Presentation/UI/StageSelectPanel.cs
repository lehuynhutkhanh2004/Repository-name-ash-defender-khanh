using AshDefender.Features.Stage.Application.Interfaces;
using AshDefender.Features.Stage.Application.UseCases;
using AshDefender.Shared.Core;
using UnityEngine;
using VContainer;

namespace AshDefender.Features.Stage.Presentation.UI
{
    public class StageSelectPanel : MonoBehaviour
    {
        private IStageService _stageService;
        private StartStageUseCase _startStageUseCase;
        private SceneLoader _sceneLoader;

        [Inject]
        public void Construct(IStageService stageService, StartStageUseCase startStageUseCase, SceneLoader sceneLoader)
        {
            _stageService = stageService;
            _startStageUseCase = startStageUseCase;
            _sceneLoader = sceneLoader;
        }

        private void OnEnable()
        {
            RefreshStageList();
        }

        private void RefreshStageList()
        {
            var stages = _stageService.GetAllStages();
        }

        public void OnStageSelected(string stageId)
        {
            try
            {
                _startStageUseCase.Execute(stageId);
                _sceneLoader.LoadAsync("Gameplay").Forget();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
    }
}
