using _Game.Scripts.Buildings;
using _Game.Scripts.Manager;
using _Game.Scripts.Scriptable_Object;
using System;
using UnityEngine;

namespace _Game.Scripts.Character.Hero
{
    public class HeroController : CharacterController
    {
        public bool IsInCombat;
        public bool IsReviving;
        public bool IsNeedHeal;

        public Action OnChangeEquipment;

        public override void Die()
        {
            base.Die();

             MoveToRevivePoint();
        }

        public void GainExp(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            HeroDataSO.CharacterStat.CurrentExp += amount;

            if (HeroDataSO.CharacterStat.CurrentExp >= HeroDataSO.CharacterStat.ExpToLevelUp)
            {
                HeroDataSO.CharacterStat.CurrentExp = HeroDataSO.CharacterStat.ExpToLevelUp;
            }
            HeroManager.Instance.SaveDataHero();
        }

        private void MoveToRevivePoint()
        {
            Vector3 revivePoint = HouseRevive.Instance.ReviveHeroPoint.position;

            while (Vector3.Distance(transform.position, revivePoint) > 0.1f)
            {
                MoveTowards(revivePoint);
                AttackTarget = null;
            }
            IsReviving = false;
            HouseRevive.Instance.ReviveHero(this);
        }

        public void SetHeroData(HeroDataSO heroData)
        {
            HeroDataSO = heroData;
            HeroDataSO.HeroName = heroData.HeroName;
            HeroDataSO.HeroID = heroData.HeroID;
            HeroDataSO.HeroAvatar = heroData.HeroAvatar;
            HeroDataSO.HeroAvatarPath = heroData.HeroAvatarPath;
            HeroDataSO.IconAvatar = heroData.IconAvatar;
            HeroDataSO.IconAvatarPath = heroData.IconAvatarPath;
            HeroDataSO.CharacterStat = heroData.CharacterStat;
            HeroDataSO.Rarity = heroData.Rarity;
            HeroDataSO.Elemental = heroData.Elemental;
        }
    }
}
