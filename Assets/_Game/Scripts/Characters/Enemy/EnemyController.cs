using _Game.Scripts.Scriptable_Object;
using _Game.Scripts.Manager;
using _Game.Scripts.Character;
using _Game.Scripts.StatePatern;
namespace _Game.Scripts.Character.Eenmy
{
    public class EnemyController : CharacterController
    {
        private void OnEnable()
        {
            ResetHeroState();
        }

      /*  public override void TryAttack()
        {
            base.TryAttack();
        }*/

        public override void Die()
        {
            base.Die();
            SpawnEnemyManager.Instance.ReleaseEnemy(this);
            Destroy(BaseRoot);
        }

        private void ResetHeroState()
        {
            SetState(new PatrolState());
            CurrentState.EnterState(this);
        }

        public void SetHeroData(HeroDataSO heroData)
        {
            HeroDataSO = heroData;
            CurrentStat = heroData.CharacterStat;
            CurrentHP = CurrentStat.Hp;
        }
    }
}
