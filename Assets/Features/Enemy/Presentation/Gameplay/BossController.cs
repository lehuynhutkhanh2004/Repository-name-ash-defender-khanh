using AshDefender.Features.Enemy.Domain.Enums;
using AshDefender.Shared.Core;
using AshDefender.Shared.Events;
using UnityEngine;
using VContainer;

namespace AshDefender.Features.Enemy.Presentation.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
    public class BossController : MonoBehaviour
    {
        [SerializeField] private string bossId;
        [SerializeField] private string bossName;
        [SerializeField] private float phase2SpeedMultiplier = 1.3f;
        [SerializeField] private float enrageSpeedMultiplier = 1.8f;

        private IEventBus _eventBus;
        private Rigidbody2D _rb;
        private Animator _animator;
        private BossPhase _currentPhase = BossPhase.Phase1;
        private int _currentHealth;
        private int _maxHealth;
        private float _baseMoveSpeed;

        private static readonly int PhaseHash = Animator.StringToHash("Phase");
        private static readonly int AttackHash = Animator.StringToHash("Attack");

        [Inject]
        public void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
        }

        public void Initialize(int health, float moveSpeed)
        {
            _maxHealth = health;
            _currentHealth = health;
            _baseMoveSpeed = moveSpeed;
            _eventBus.Publish(new BossSpawnedEvent(bossId, bossName));
        }

        public void TakeDamage(int damage)
        {
            _currentHealth = System.Math.Max(0, _currentHealth - damage);
            UpdatePhase();
            if (_currentHealth <= 0) Die();
        }

        private void UpdatePhase()
        {
            var ratio = (float)_currentHealth / _maxHealth;
            var newPhase = ratio switch
            {
                <= 0.25f => BossPhase.Enrage,
                <= 0.6f => BossPhase.Phase2,
                _ => BossPhase.Phase1
            };

            if (newPhase == _currentPhase) return;
            _currentPhase = newPhase;
            _animator.SetInteger(PhaseHash, (int)_currentPhase);
        }

        private void Die()
        {
            _eventBus.Publish(new BossDefeatedEvent(bossId));
            Destroy(gameObject, 1.5f);
        }
    }
}
