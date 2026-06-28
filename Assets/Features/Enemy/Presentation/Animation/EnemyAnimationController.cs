using AshDefender.Features.Enemy.Domain.Enums;
using UnityEngine;

namespace AshDefender.Features.Enemy.Presentation.Animation
{
    [RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class EnemyAnimationController : MonoBehaviour
    {
        private static readonly int StateHash = Animator.StringToHash("State");
        private static readonly int AttackHash = Animator.StringToHash("Attack");

        private Animator _animator;
        private SpriteRenderer _sprite;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _sprite = GetComponent<SpriteRenderer>();
        }

        public void SetState(EnemyState state) => _animator.SetInteger(StateHash, (int)state);

        public void TriggerAttack() => _animator.SetTrigger(AttackHash);

        public void FaceDirection(float dirX) => _sprite.flipX = dirX > 0f;
    }
}
