using _Game.Scripts.StatePatern;

namespace _Game.Scripts.Character
{
    public class IdleState : ICharacterState
    {
        public void EnterState(CharacterController character)
        {
           /* if (character.Animator != null)
            {
                character.Animator.Play("Idle");
            }*/
        }

        public void UpdateState(CharacterController character)
        {
            if (character.IsAttack)
            {
                character.SetState(new AttackState());
            }
            else if (character.HasPatrolPoints())
            {
                character.SetState(new PatrolState());
            }
        }

        public void ExitState(CharacterController character) { }
    }
}
