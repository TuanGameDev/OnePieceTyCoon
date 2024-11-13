
using _Game.Scripts.Character;

namespace _Game.Scripts.StatePatern
{
    public class AttackState : ICharacterState
    {
        public void EnterState(CharacterController character)
        {
            character.Animator.SetTrigger("Attack");
        }

        public void UpdateState(CharacterController character)
        {
            if (!character.IsAttack || character.AttackTarget == null || character.AttackTarget.gameObject == null)
            {
                character.IsAttack = false;
                character.AttackTarget = null;
                character.SetState(new IdleState());
                return;
            }

            character.TryAttack();
        }


        public void ExitState(CharacterController character) { }
    }
}
