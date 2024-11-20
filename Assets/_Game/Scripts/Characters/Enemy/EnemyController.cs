using _Game.Scripts.Scriptable_Object;
using _Game.Scripts.Manager;
using _Game.Scripts.StatePatern;
using UnityEngine;
using System;
using _Game.Scripts.Character.Hero;
using _Game.Scripts.Scriptable_Objects;

namespace _Game.Scripts.Character.Eenmy
{
    public class EnemyController : CharacterController
    {
        [Space(10)]
        [Header("Drop")]
        public int ExpReward;

        [SerializeField]
        private ItemObject _itemPrefab;

        public DropItemDataSO _droppable;
        private void OnEnable()
        {
            ResetHeroState();
        }

        public override void Die()
        {
            base.Die();
            SpawnEnemyManager.Instance.ReleaseEnemy(this);
            Destroy(BaseRoot);

            var hero = AttackTarget as HeroController;
            if (hero != null)
            {
                hero.GainExp(ExpReward);
            }
            Vector2 dropPosition = transform.position;
            _droppable.DropItemsByRate(_itemPrefab, dropPosition);
            OnDie?.Invoke();
        }

        private void ResetHeroState()
        {
            Invoke(nameof(UpdateAnimation), 0.1f);
            SetState(new PatrolState());
            CurrentState.EnterState(this);
        }

        private void UpdateAnimation()
        {
            if (Animations != null)
            {
                Animations.SetAnimator(Animator);
            }
        }

        public void SetHeroData(HeroDataSO heroData)
        {
            HeroDataSO = heroData;
            CurrentStat = heroData.CharacterStat;
            CurrentHP = CurrentStat.Hp;
            AttackTarget = null;
        }
    }
}
