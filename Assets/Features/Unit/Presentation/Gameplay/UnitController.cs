using AshDefender.Features.Unit.Domain.Enums;
using AshDefender.Shared.Constants;
using UnityEngine;

namespace AshDefender.Features.Unit.Presentation.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
    public class UnitController : MonoBehaviour
    {
        [SerializeField] private string unitId;
        [SerializeField] private float attackRange = 1.5f;

        private Rigidbody2D _rb;
        private Animator _animator;
        private UnitState _state = UnitState.Idle;
        private Transform _target;
        private float _moveSpeed;

        private static readonly int StateHash = Animator.StringToHash("State");
        private static readonly int AttackHash = Animator.StringToHash("Attack");

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
        }

        public void Initialize(float moveSpeed)
        {
            _moveSpeed = moveSpeed;
        }

        private void Update()
        {
            switch (_state)
            {
                case UnitState.Idle: FindTarget(); break;
                case UnitState.Move: MoveToTarget(); break;
                case UnitState.Attack: PerformAttack(); break;
            }
        }

        private void FindTarget()
        {
            var hit = Physics2D.OverlapCircle(transform.position, attackRange * 5f,
                LayerMask.GetMask(LayerConstants.Enemy));
            if (hit != null)
            {
                _target = hit.transform;
                ChangeState(UnitState.Move);
            }
        }

        private void MoveToTarget()
        {
            if (_target == null) { ChangeState(UnitState.Idle); return; }

            var dir = ((Vector2)(_target.position - transform.position)).normalized;
            _rb.linearVelocity = dir * _moveSpeed;

            if (Vector2.Distance(transform.position, _target.position) <= attackRange)
            {
                _rb.linearVelocity = Vector2.zero;
                ChangeState(UnitState.Attack);
            }
        }

        private void PerformAttack()
        {
            if (_target == null) { ChangeState(UnitState.Idle); return; }
            _animator.SetTrigger(AttackHash);
        }

        public void Die()
        {
            ChangeState(UnitState.Dead);
            _rb.linearVelocity = Vector2.zero;
            enabled = false;
        }

        private void ChangeState(UnitState newState)
        {
            _state = newState;
            _animator.SetInteger(StateHash, (int)newState);
        }
    }
}
