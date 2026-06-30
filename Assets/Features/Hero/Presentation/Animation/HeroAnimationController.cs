using AshDefender.Features.Hero.Domain.Enums;
using UnityEngine;

namespace AshDefender.Features.Hero.Presentation.Animation
{
    [RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class HeroAnimationController : MonoBehaviour
    {
        private static readonly int StateHash = Animator.StringToHash("State");
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int SkillHash = Animator.StringToHash("CastSkill");

        private Animator _animator;
        private SpriteRenderer _sprite;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _sprite = GetComponent<SpriteRenderer>();
        }

        public void SetState(HeroState state) => _animator.SetInteger(StateHash, (int)state);

        public void TriggerAttack() => _animator.SetTrigger(AttackHash);

        public void TriggerSkill() => _animator.SetTrigger(SkillHash);

        public void FaceDirection(float directionX) =>
            _sprite.flipX = directionX < 0f;
    }
}
