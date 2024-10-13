using _Game.Scripts.Enums;
using System;

namespace _Game.Scripts.Non_Mono
{
    [System.Serializable]
    public class CharacterNameAndRank
    {
        public CharacterNameAndRank(CharacterName herotype, Rank rarity)
        {
            CharacterName = herotype;
            Rank = rarity;
        }
        public CharacterNameAndRank(CharacterNameAndRank data)
        {
            CharacterName = data.CharacterName;
            Rank = data.Rank;
        }

        public CharacterName CharacterName;
        public Rank Rank;

        public static bool operator ==(CharacterNameAndRank a, CharacterNameAndRank b)
        {
            return a?.CharacterName == b?.CharacterName && a?.Rank == b?.Rank;
        }
        
        public static bool operator !=(CharacterNameAndRank a, CharacterNameAndRank b)
        {
            return a?.CharacterName != b?.CharacterName || a?.Rank != b?.Rank;
        }
        public override bool Equals(object obj)
        {
            if (obj is CharacterNameAndRank other)
            {
                return CharacterName == other.CharacterName && Rank == other.Rank;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CharacterName, Rank);
        }
    }
}