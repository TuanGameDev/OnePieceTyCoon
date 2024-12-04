using _Game.Scripts.Scriptable_Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace _Game.Scripts.Character.Hero
{
    public class BossController : CharacterController
    {
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
