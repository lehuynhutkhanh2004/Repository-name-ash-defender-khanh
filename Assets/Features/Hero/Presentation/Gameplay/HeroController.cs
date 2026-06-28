using AshDefender.Features.Hero.Domain.Enums;
using AshDefender.Shared.Core;
using AshDefender.Shared.Events;
using UnityEngine;
using VContainer;

namespace AshDefender.Features.Hero.Presentation.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
    public class HeroController : MonoBehaviour
    {
        [SerializeField] private string heroId;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float moveSpeed = 3f;

        private IEventBus _eventBus;
        private Rigidbody2D _rb;
        private Animator _animator;
        private HeroState _state = HeroState.Idle;
        private Transform _target;

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

        private void Start()
        {
            _eventBus.Publish(new HeroSpawnedEvent(heroId));
        }

        private void Update()
        {
            switch (_state)
            {
                case HeroState.Idle: FindTarget(); break;
                case HeroState.Move: MoveToTarget(); break;
                case HeroState.Attack: PerformAttack(); break;
            }
        }

        private void FindTarget()
        {
            var hit = Physics2D.OverlapCircle(transform.position, attackRange * 3f,
                LayerMask.GetMask("Enemy"));
            if (hit != null)
            {
                _target = hit.transform;
                ChangeState(HeroState.Move);
            }
        }

        private void MoveToTarget()
        {
            if (_target == null) { ChangeState(HeroState.Idle); return; }

            var dir = ((Vector2)(_target.position - transform.position)).normalized;
            _rb.linearVelocity = dir * moveSpeed;

            if (Vector2.Distance(transform.position, _target.position) <= attackRange)
            {
                _rb.linearVelocity = Vector2.zero;
                ChangeState(HeroState.Attack);
            }
        }

        private void PerformAttack()
        {
            if (_target == null) { ChangeState(HeroState.Idle); return; }
            _animator.SetTrigger("Attack");
        }

        private void ChangeState(HeroState newState)
        {
            _state = newState;
            _animator.SetInteger("State", (int)newState);
        }

        public void Die()
        {
            ChangeState(HeroState.Dead);
            _rb.linearVelocity = Vector2.zero;
            enabled = false;
        }
    }
}
