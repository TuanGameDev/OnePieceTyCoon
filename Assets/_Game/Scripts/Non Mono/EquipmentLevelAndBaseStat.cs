using _Game.Scripts.Characters;
using _Game.Scripts.Enums;
using _Game.Scripts.Helper;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Non_Mono
{

    [System.Serializable]
    public class Equipment
    {
        public int LevelEquipment;
        public EquipmentType EquipmentType;
        public List<LevelEquip> LevelStat;
    }
    [System.Serializable]
    public class LevelEquip
    {
        public int Level;
        public Rarity Rarity;
        public Sprite IconItem;
        public string IconEquipmentPath;
    }
}