using _Game.Scripts.Characters;
using _Game.Scripts.Enums;
using UnityEngine;
namespace _Game.Scripts.Scriptable_Object
{
    [CreateAssetMenu(fileName = "NewHero", menuName = "_Game/Character/HeroDataSO", order = 0)]
    public class HeroDataSO : ScriptableObject
    {
        public int HeroID;
        public Sprite HeroAvatar;
        public Sprite IconAvatar;
        public string HeroAvatarPath;
        public string IconAvatarPath;
        public int Power;

        [Header("Stat Hero")]
        public CharacterStat CharacterStat;
        public CharacterName CharacterName;
        public Rarity Rarity;
        public Elemental Elemental;
    }
    [System.Serializable]
    public class HeroData
    {
        public int HeroID;
        public Sprite HeroAvatar;
        public Sprite IconAvatar;
        public string HeroAvatarPath;
        public string IconAvatarPath;
        public int Power;

        [Header("Stat Hero")]
        public CharacterStat CharacterStat;
        public CharacterName CharacterName;
        public Rarity Rarity;
        public Elemental Elemental;

    }

    public class HeroDataConverter
    {
        public static HeroData ConvertHeroDataSOToHeroData(HeroDataSO heroDataSO)
        {
            HeroData heroData = new HeroData
            {
              HeroID = heroDataSO.HeroID,
              HeroAvatar = heroDataSO.HeroAvatar,
              IconAvatar = heroDataSO.IconAvatar,
              HeroAvatarPath = heroDataSO.HeroAvatarPath,
              IconAvatarPath = heroDataSO.IconAvatarPath,
              Power = heroDataSO.Power,
              CharacterStat = heroDataSO.CharacterStat,
              CharacterName = heroDataSO.CharacterName,
              Rarity = heroDataSO.Rarity,
              Elemental = heroDataSO.Elemental,
            };
            return heroData;
        }
    }

}
