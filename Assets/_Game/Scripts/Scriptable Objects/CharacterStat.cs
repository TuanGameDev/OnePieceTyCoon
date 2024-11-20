namespace _Game.Scripts.Characters
{
    [System.Serializable]
    public class CharacterStat
    {
        public int HeroLevel;
        public int Hp;
        public int Def;
        public float MoveSpeed;
        public int AttackDamage;
        public int CurrentExp;
        public int ExpToLevelUp;
        public CharacterStat Clone()
        {
            return new CharacterStat
            {
                HeroLevel = this.HeroLevel,
                Hp = this.Hp,
                Def = this.Def,
                MoveSpeed = this.MoveSpeed,
                AttackDamage = this.AttackDamage,
                CurrentExp = this.CurrentExp,
                ExpToLevelUp = this.ExpToLevelUp
            };
        }
    }
}
