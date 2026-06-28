using AshDefender.Shared.Core;
using AshDefender.Shared.Events;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace AshDefender.Features.SaveSystem.Presentation.UI
{
    public class SaveIndicatorUI : MonoBehaviour
    {
        [SerializeField] private GameObject indicator;
        [SerializeField] private float displayDuration = 2f;

        private IEventBus _eventBus;
        private System.IDisposable _subscription;

        [Inject]
        public void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void OnEnable()
        {
            _subscription = _eventBus.Subscribe<GameSavedEvent>(OnGameSaved);
        }

        private void OnDisable()
        {
            _subscription?.Dispose();
        }

        private void OnGameSaved(GameSavedEvent e) => ShowAsync().Forget();

        private async UniTaskVoid ShowAsync()
        {
            if (indicator != null) indicator.SetActive(true);
            await UniTask.Delay(System.TimeSpan.FromSeconds(displayDuration));
            if (indicator != null) indicator.SetActive(false);
        }
    }
}
