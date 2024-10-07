using _Game.Scripts.Characters;
using _Game.Scripts.Enums;
using UnityEngine;
namespace _Game.Scripts.Scriptable_Object
{
    [CreateAssetMenu(fileName = "NewHero", menuName = "_Game/Character/HeroDataSO", order = 0)]
    public class HeroDataSO : ScriptableObject
    {
        public int HeroID;
        public string HeroName;
        public Sprite HeroAvatar;
        public string AvatarPath;

        [Header("Stat Hero")]
        public CharacterStat CharacterStat;
        public Rarity Rarity;
    }
    [System.Serializable]
    public class HeroData
    {
        public int HeroID;
        public Sprite HeroAvatar;
        public string HeroName;
        public string AvatarPath;
        public CharacterStat CharacterStat;
        public Rarity Rarity;
    }

    public class HeroDataConverter
    {
        public static HeroData ConvertHeroDataSOToHeroData(HeroDataSO heroDataSO)
        {
            HeroData heroData = new HeroData
            {
              HeroID = heroDataSO.HeroID,
              HeroName=heroDataSO.HeroName,
              HeroAvatar = heroDataSO.HeroAvatar,
              AvatarPath = heroDataSO.AvatarPath,
              CharacterStat = heroDataSO.CharacterStat,
              Rarity = heroDataSO.Rarity,
            };
            return heroData;
        }
    }

}
