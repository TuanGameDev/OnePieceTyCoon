using UnityEngine;

namespace _Game.Scripts.Characters
{
    public class CharacterAnimations : MonoBehaviour
    {
        private Animator _animator;

        private readonly int IDLE = Animator.StringToHash("Idle");
        private readonly int MOVE = Animator.StringToHash("Move");
        private readonly int DIE = Animator.StringToHash("Die");

        private float _transitionDuration = 0.1f;

        public void SetAnimator(Animator animator)
        {
            _animator = animator;
        }

        public void PlayIdle()
        {
            if (_animator != null)
                _animator.CrossFadeInFixedTime(IDLE, _transitionDuration);
        }

        public void PlayMove()
        {
            if (_animator != null)
                _animator.CrossFadeInFixedTime(MOVE, _transitionDuration);
        }

        public void PlayDead()
        {
            if (_animator != null)
                _animator.CrossFadeInFixedTime(DIE, _transitionDuration);
        }

        public void PlayAttack(string attackName)
        {
            if (_animator != null)
                _animator.CrossFadeInFixedTime(attackName, _transitionDuration);
        }
    }
}
