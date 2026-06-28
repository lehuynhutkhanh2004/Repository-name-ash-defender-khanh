using AshDefender.Features.CoreDefense.Domain.Interfaces;
using AshDefender.Shared.Core;
using AshDefender.Shared.Events;
using UnityEngine;
using VContainer;

namespace AshDefender.Features.CoreDefense.Presentation.Gameplay
{
    public class CoreController : MonoBehaviour
    {
        [SerializeField] private int maxHP = 100;

        private ICoreRepository _coreRepository;
        private IEventBus _eventBus;

        [Inject]
        public void Construct(ICoreRepository coreRepository, IEventBus eventBus)
        {
            _coreRepository = coreRepository;
            _eventBus = eventBus;
        }

        public void TakeDamage(int damage)
        {
            var core = _coreRepository.GetCore();
            var actual = core.TakeDamage(damage);
            _coreRepository.Save(core);

            _eventBus.Publish(new CoreDamagedEvent(actual, core.CurrentHP, core.MaxHP));

            if (core.IsDestroyed)
                OnCoreDestroyed();
        }

        private void OnCoreDestroyed()
        {
            Debug.Log("Core destroyed — game over.");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
                TakeDamage(10);
        }
    }
}
