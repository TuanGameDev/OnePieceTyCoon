using _Game.Scripts.Scriptable_Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Character.Hero
{
    public class HeroController : CharacterController
    {
        public override void Attack()
        {
            base.Attack();
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
