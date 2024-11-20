using Cysharp.Threading.Tasks;
using UnityEngine.TextCore.Text;
using UnityEngine;
using _Game.Scripts.StatePatern;
namespace _Game.Scripts.Character
{

    public class PatrolState : ICharacterState
    {
        private Transform _targetPoint;
        private bool _isWaiting;
        private float _detectedDistance = 3f;
        private float _attackDistance = 1.5f;

        public void EnterState(Character.CharacterController character)
        {
            _targetPoint = character.GetRandomPatrolPoint();

            if (character.Animations != null)
            {
                character.Animations.PlayMove();
            }

            _isWaiting = false;

            if (_targetPoint != null)
            {
                FlipTowardsTargetPoint(character, _targetPoint.position);
            }
        }

        public async void UpdateState(Character.CharacterController character)
        {
            if (_isWaiting)
            {
                return;
            }

            if (DetectedCharacter(character))
            {
                MoveTowardsTarget(character);
                return;
            }

            if (_targetPoint == null)
            {
                character.SetState(new IdleState());
                return;
            }

            character.MoveTowards(_targetPoint.position);

            if (Vector3.Distance(character.transform.position, _targetPoint.position) < 0.1f)
            {
                if (character.Animations != null)
                {
                    character.Animations.PlayIdle();
                }

                _isWaiting = true;
                await UniTask.Delay(2000);

                _targetPoint = character.GetRandomPatrolPoint();

                if (character.Animations != null)
                {
                    character.Animations.PlayMove();
                }

                if (_targetPoint != null)
                {
                    FlipTowardsTargetPoint(character, _targetPoint.position);
                }

                _isWaiting = false;
            }
        }

        public void ExitState(Character.CharacterController character)
        {
            if (character.Animations != null)
            {
                character.Animations.PlayIdle();
            }
        }

        private bool DetectedCharacter(Character.CharacterController character)
        {
            Collider2D[] detectedEnemies = Physics2D.OverlapCircleAll(character.transform.position, _detectedDistance, character.Layer);

            foreach (var enemy in detectedEnemies)
            {
                Character.CharacterController detectedCharacter = enemy.GetComponent<Character.CharacterController>();

                if (detectedCharacter != null && detectedCharacter != character)
                {
                    character.AttackTarget = detectedCharacter;
                    character.IsAttack = true;
                    return true;
                }
            }

            character.AttackTarget = null;
            character.IsAttack = false;
            return false;
        }

        private void MoveTowardsTarget(Character.CharacterController character)
        {
            if (character.AttackTarget == null) return;

            float distanceToTarget = Vector3.Distance(character.transform.position, character.AttackTarget.transform.position);

            if (distanceToTarget > _attackDistance)
            {
                character.MoveTowards(character.AttackTarget.transform.position);
                FlipTowardsTargetPoint(character, character.AttackTarget.transform.position);
            }
            else
            {
                character.SetState(new IdleState());
            }
        }

        private void FlipTowardsTargetPoint(Character.CharacterController character, Vector3 targetPosition)
        {
            if (character != null)
            {
                bool shouldFlipRight = targetPosition.x > character.transform.position.x;
                bool shouldFlipLeft = targetPosition.x < character.transform.position.x;

                if (shouldFlipRight && !character.FaceRight)
                {
                    character.FlipRight();
                }
                else if (shouldFlipLeft && character.FaceRight)
                {
                    character.FlipLeft();
                }
            }
        }
    }
}
