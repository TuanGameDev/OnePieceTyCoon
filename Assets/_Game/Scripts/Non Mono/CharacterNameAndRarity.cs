using _Game.Scripts.Enums;
using System;

namespace _Game.Scripts.Non_Mono
{
    [System.Serializable]
    public class CharacterNameAndRarity
    {
        public CharacterNameAndRarity(CharacterName herotype, Rarity rarity)
        {
            CharacterName = herotype;
            Rarity = rarity;
        }
        public CharacterNameAndRarity(CharacterNameAndRarity data)
        {
            CharacterName = data.CharacterName;
            Rarity = data.Rarity;
        }

        public CharacterName CharacterName;
        public Rarity Rarity;

        public static bool operator ==(CharacterNameAndRarity a, CharacterNameAndRarity b)
        {
            return a?.CharacterName == b?.CharacterName && a?.Rarity == b?.Rarity;
        }
        
        public static bool operator !=(CharacterNameAndRarity a, CharacterNameAndRarity b)
        {
            return a?.CharacterName != b?.CharacterName || a?.Rarity != b?.Rarity;
        }
        public override bool Equals(object obj)
        {
            if (obj is CharacterNameAndRarity other)
            {
                return CharacterName == other.CharacterName && Rarity == other.Rarity;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CharacterName, Rarity);
        }
    }
}