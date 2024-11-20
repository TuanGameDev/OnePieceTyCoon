using _Game.Scripts.Characters;
using _Game.Scripts.Enums;
using _Game.Scripts.Non_Mono;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        public List<LevelStats> LevelStats;
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
        public List<LevelStats> LevelStats;
    }
}
