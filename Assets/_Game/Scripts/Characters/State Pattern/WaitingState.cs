using _Game.Scripts.StatePatern;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

namespace _Game.Scripts.Character
{
    public class WaitingState : ICharacterState
    {
        private bool _hasWaited;

        public async void EnterState(CharacterController character)
        {
            _hasWaited = false;

            if (character.Animations != null)
            {
                character.Animations.PlayIdle();
            }

            await UniTask.Delay(3000);
            _hasWaited = true;
        }

        public void UpdateState(CharacterController character)
        {
            if (_hasWaited)
            {
                character.SetState(new PatrolState());
            }
        }

        public void ExitState(CharacterController character) { }
    }
}
