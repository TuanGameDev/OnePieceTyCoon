using _Game.Scripts.Characters;
using _Game.Scripts.Enums;
using _Game.Scripts.Non_Mono;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
namespace _Game.Scripts.Scriptable_Object
{
    [CreateAssetMenu(fileName = "NewHero", menuName = "_Game/Character/HeroDataSO", order = 0)]
    public class HeroDataSO : ScriptableObject
    {
        public int HeroID;
        public string HeroName;
        public Sprite HeroAvatar;
        public Sprite IconAvatar;
        public string HeroAvatarPath;
        public string IconAvatarPath;
        public int Power;

        [Header("Stat Hero")]
        public CharacterStat CharacterStat;
        public HeroType HeroType;
        public Rarity Rarity;
        public Elemental Elemental;
        public List<LevelStats> LevelStats;
        public List<Equipment> Equipment;
    }
    [System.Serializable]
    public class HeroData
    {
        public int HeroID;
        public string HeroName;
        public Sprite HeroAvatar;
        public Sprite IconAvatar;
        public string HeroAvatarPath;
        public string IconAvatarPath;
        public int Power;

        [Header("Stat Hero")]
        public CharacterStat CharacterStat;
        public HeroType HeroType;
        public Rarity Rarity;
        public Elemental Elemental;
        public List<LevelStats> LevelStats;
        public List<Equipment> Equipment;

        [Button("Clear")]
        public void Clear()
        {
            HeroID = 0;
            HeroName = null;
            Power = 0;
            HeroAvatar = null;
            IconAvatar = null;
            HeroAvatarPath = null;
            IconAvatarPath = null;
            CharacterStat = null;
            Rarity.C.ToString();
            Elemental.None.ToString();
            LevelStats = null;
            Equipment = null;

    }
}
}
