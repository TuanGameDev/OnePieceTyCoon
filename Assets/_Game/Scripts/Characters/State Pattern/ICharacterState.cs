using _Game.Scripts.Character;

namespace _Game.Scripts.StatePatern
{
    public interface ICharacterState
    {
        void EnterState(CharacterController character);
        void UpdateState(CharacterController character);
        void ExitState(CharacterController character);
    }
}
