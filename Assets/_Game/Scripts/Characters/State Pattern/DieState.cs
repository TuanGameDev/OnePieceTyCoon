using _Game.Scripts.Character;
using UnityEngine;

namespace _Game.Scripts.StatePatern
{
    public class DieState : ICharacterState
    {
        public void EnterState(Character.CharacterController character)
        {
            character.Animations.PlayDead();
            character.IsDead = true;
            character.AttackTarget = null;
            character.GetComponent<BoxCollider2D>().enabled = false;

        }

        public void UpdateState(Character.CharacterController character) { }

        public void ExitState(Character.CharacterController character) { }
    }
}
