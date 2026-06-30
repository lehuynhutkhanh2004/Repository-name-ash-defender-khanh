using AshDefender.Shared.Core;
using AshDefender.Shared.Events;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace AshDefender.Features.CoreDefense.Presentation.UI
{
    public class CoreHealthUI : MonoBehaviour
    {
        private IEventBus _eventBus;
        private ProgressBar _healthBar;
        private Label _hpLabel;
        private System.IDisposable _subscription;

        [Inject]
        public void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void OnEnable()
        {
            _subscription = _eventBus.Subscribe<CoreDamagedEvent>(OnCoreDamaged);
        }

        private void OnDisable()
        {
            _subscription?.Dispose();
        }

        private void OnCoreDamaged(CoreDamagedEvent e)
        {
            if (_healthBar != null)
                _healthBar.value = e.MaxHP > 0 ? (float)e.CurrentHP / e.MaxHP * 100f : 0f;
            if (_hpLabel != null)
                _hpLabel.text = $"{e.CurrentHP} / {e.MaxHP}";
        }
    }
}
