using _Game.Scripts.Enums;
using System;

namespace _Game.Scripts.Non_Mono
{
    [System.Serializable]
    public class CharacterState
    {
        public string CharacterName;
        public Rarity Rarity;
        public Elemental Elemental;

        public CharacterState(string characterName, Rarity rarity, Elemental elemental)
        {
            CharacterName = characterName;
            Rarity = rarity;
            Elemental = elemental;
        }

        public CharacterState(CharacterState data)
        {
            CharacterName = data.CharacterName;
            Rarity = data.Rarity;
            Elemental = data.Elemental;
        }

        public override bool Equals(object obj)
        {
            if (obj is CharacterState other)
            {
                return CharacterName == other.CharacterName && Rarity == other.Rarity && Elemental == other.Elemental;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CharacterName, Rarity, Elemental);
        }
    }
}
