using AshDefender.Shared.Core;
using AshDefender.Shared.Events;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace AshDefender.Features.Hero.Presentation.UI
{
    public class HeroStatusUI : MonoBehaviour
    {
        [SerializeField] private string heroId;

        private IEventBus _eventBus;
        private ProgressBar _healthBar;
        private Label _levelLabel;
        private System.IDisposable _subscription;

        [Inject]
        public void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void OnEnable()
        {
            _subscription = _eventBus.Subscribe<HeroUpgradedEvent>(OnHeroUpgraded);
        }

        private void OnDisable()
        {
            _subscription?.Dispose();
        }

        public void SetHealth(int current, int max)
        {
            if (_healthBar == null) return;
            _healthBar.value = max > 0 ? (float)current / max * 100f : 0f;
        }

        private void OnHeroUpgraded(HeroUpgradedEvent e)
        {
            if (e.HeroId != heroId) return;
            if (_levelLabel != null)
                _levelLabel.text = $"Lv.{e.NewLevel}";
        }
    }
}
