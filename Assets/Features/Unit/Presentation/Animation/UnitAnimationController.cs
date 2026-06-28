using AshDefender.Features.Unit.Domain.Enums;
using UnityEngine;

namespace AshDefender.Features.Unit.Presentation.Animation
{
    [RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class UnitAnimationController : MonoBehaviour
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

        public void SetState(UnitState state) => _animator.SetInteger(StateHash, (int)state);

        public void TriggerAttack() => _animator.SetTrigger(AttackHash);

        public void FaceDirection(float dirX) => _sprite.flipX = dirX < 0f;
    }
}
