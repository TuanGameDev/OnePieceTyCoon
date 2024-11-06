using _Game.Scripts.Enums;
using System;

namespace _Game.Scripts.Non_Mono
{
    [System.Serializable]
    public class CharacterState
    {
        public CharacterState(CharacterName characternname, Rarity rarity, Elemental elemental)
        {
            CharacterName = characternname;
            Rarity = rarity;
            Elemental = elemental;
        }
        public CharacterState(CharacterState data)
        {
            CharacterName = data.CharacterName;
            Rarity = data.Rarity;
            Elemental = data.Elemental;
        }

        public CharacterName CharacterName;
        public Rarity Rarity;
        public Elemental Elemental;
    }
}