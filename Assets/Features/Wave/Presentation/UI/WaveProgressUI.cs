using AshDefender.Shared.Core;
using AshDefender.Shared.Events;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace AshDefender.Features.Wave.Presentation.UI
{
    public class WaveProgressUI : MonoBehaviour
    {
        private IEventBus _eventBus;
        private Label _waveLabel;
        private System.IDisposable _stageStartedSub;

        [Inject]
        public void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void OnEnable()
        {
            _stageStartedSub = _eventBus.Subscribe<StageStartedEvent>(OnStageStarted);
        }

        private void OnDisable()
        {
            _stageStartedSub?.Dispose();
        }

        public void SetWave(int current, int total)
        {
            if (_waveLabel != null)
                _waveLabel.text = $"Wave {current} / {total}";
        }

        private void OnStageStarted(StageStartedEvent e)
        {
            if (_waveLabel != null)
                _waveLabel.text = "Wave 1";
        }
    }
}
