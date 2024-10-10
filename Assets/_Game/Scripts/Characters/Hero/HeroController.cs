using _Game.Scripts.Scriptable_Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Character.Hero
{
    public class HeroController : CharacterController
    {
        public GameObject BaseRoot;

        public override void Attack()
        {
            base.Attack();
        }

        public void SetHeroData(HeroDataSO heroData)
        {
            HeroDataSO = heroData;
            HeroDataSO.HeroName = heroData.HeroName;
            HeroDataSO.HeroID = heroData.HeroID;
            HeroDataSO.CharacterStat = heroData.CharacterStat;
            HeroDataSO.CharacterName = heroData.CharacterName;
            HeroDataSO.Rarity = heroData.Rarity;
        }
    }
}
