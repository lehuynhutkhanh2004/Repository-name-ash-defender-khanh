using AshDefender.Features.Enemy.Domain.Enums;
using AshDefender.Shared.Constants;
using AshDefender.Shared.Core;
using AshDefender.Shared.Events;
using UnityEngine;
using VContainer;

namespace AshDefender.Features.Enemy.Presentation.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private string enemyId;
        [SerializeField] private float attackRange = 1f;

        private IEventBus _eventBus;
        private Rigidbody2D _rb;
        private Animator _animator;
        private EnemyState _state = EnemyState.Move;
        private Transform _coreTarget;
        private float _moveSpeed;
        private int _goldReward;

        private static readonly int StateHash = Animator.StringToHash("State");
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

        public void Initialize(float moveSpeed, int goldReward)
        {
            _moveSpeed = moveSpeed;
            _goldReward = goldReward;

            var core = GameObject.FindWithTag(TagConstants.Core);
            if (core != null) _coreTarget = core.transform;

            _eventBus.Publish(new EnemySpawnedEvent(enemyId, gameObject.name));
        }

        private void Update()
        {
            if (_state == EnemyState.Dead) return;

            if (_coreTarget == null) return;

            var dist = Vector2.Distance(transform.position, _coreTarget.position);
            if (dist <= attackRange)
            {
                ChangeState(EnemyState.Attack);
                _rb.linearVelocity = Vector2.zero;
                _animator.SetTrigger(AttackHash);
            }
            else
            {
                ChangeState(EnemyState.Move);
                var dir = ((Vector2)(_coreTarget.position - transform.position)).normalized;
                _rb.linearVelocity = dir * _moveSpeed;
            }
        }

        public void TakeDamage(int damage)
        {
            if (_state == EnemyState.Dead) return;
        }

        public void Die()
        {
            ChangeState(EnemyState.Dead);
            _rb.linearVelocity = Vector2.zero;
            _eventBus.Publish(new EnemyKilledEvent(enemyId, _goldReward));
            Destroy(gameObject, 1f);
        }

        private void ChangeState(EnemyState newState)
        {
            if (_state == newState) return;
            _state = newState;
            _animator.SetInteger(StateHash, (int)newState);
        }
    }
}
