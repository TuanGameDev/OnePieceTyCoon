using _Game.Scripts.StatePatern;
using Cysharp.Threading.Tasks;

namespace _Game.Scripts.Character
{
    public class IdleState : ICharacterState
    {
        public  void EnterState(CharacterController character)
        {
            if (character.Animator != null)
            {
                character.Animations.PlayIdle();
            }
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
