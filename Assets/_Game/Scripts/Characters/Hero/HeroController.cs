using _Game.Scripts.Buildings;
using _Game.Scripts.Scriptable_Object;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Scripts.Character.Hero
{
    public class HeroController : CharacterController
    {
        public bool IsReviving;
        public bool IsNeedHeal;

     /*   public override void TryAttack()
        {
            base.TryAttack();
        }*/

        public override void Die()
        {
            base.Die();

             MoveToRevivePoint();
        }

        private void MoveToRevivePoint()
        {
            Vector3 revivePoint = HouseRevive.Instance.ReviveHeroPoint.position;

            while (Vector3.Distance(transform.position, revivePoint) > 0.1f)
            {
                MoveTowards(revivePoint);
            }
            IsReviving = false;
            HouseRevive.Instance.ReviveHero(this);
        }

        public void SetHeroData(HeroDataSO heroData)
        {
            HeroDataSO = heroData;
            HeroDataSO.HeroID = heroData.HeroID;
            HeroDataSO.HeroAvatar = heroData.HeroAvatar;
            HeroDataSO.HeroAvatarPath = heroData.HeroAvatarPath;
            HeroDataSO.IconAvatar = heroData.IconAvatar;
            HeroDataSO.IconAvatarPath = heroData.IconAvatarPath;
            HeroDataSO.CharacterStat = heroData.CharacterStat;
            HeroDataSO.CharacterName = heroData.CharacterName;
            HeroDataSO.Rarity = heroData.Rarity;
            HeroDataSO.Elemental = heroData.Elemental;
        }
    }
}
