using AshDefender.Features.SaveSystem.Application.UseCases;
using AshDefender.Features.SaveSystem.Domain.Interfaces;
using AshDefender.Shared.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace AshDefender.Features.MainMenu.Presentation.SceneManagement
{
    public class MainMenuSceneController : MonoBehaviour
    {
        private ISaveService _saveService;
        private SceneLoader _sceneLoader;

        [Inject]
        public void Construct(ISaveService saveService, SceneLoader sceneLoader)
        {
            _saveService = saveService;
            _sceneLoader = sceneLoader;
        }

        private async void Start()
        {
            await LoadSaveDataAsync();
        }

        private async UniTask LoadSaveDataAsync()
        {
            if (_saveService.HasSave("slot_0"))
            {
                var data = await _saveService.LoadAsync("slot_0");
                Debug.Log($"[MainMenu] Save loaded. Stage: {data.CurrentStageIndex}, Gold: {data.Gold}");
            }
        }
    }
}
