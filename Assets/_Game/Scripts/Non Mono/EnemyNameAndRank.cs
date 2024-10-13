using _Game.Scripts.Enums;
using _Game.Scripts.Non_Mono;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace _Game.Scripts.Non_Mono
{
    [System.Serializable]
    public class EnemyNameAndRank
    {
        public EnemyNameAndRank(EnemysName enemyName, Rank rarity)
        {
            EnemyName = enemyName;
            Rank = rarity;
        }
        public EnemyNameAndRank(EnemyNameAndRank data)
        {
            EnemyName = data.EnemyName;
            Rank = data.Rank;
        }

        public EnemysName EnemyName;
        public Rank Rank;

        public static bool operator ==(EnemyNameAndRank a, EnemyNameAndRank b)
        {
            return a?.EnemyName == b?.EnemyName && a?.Rank == b?.Rank;
        }

        public static bool operator !=(EnemyNameAndRank a, EnemyNameAndRank b)
        {
            return a?.EnemyName != b?.EnemyName || a?.Rank != b?.Rank;
        }
        public override bool Equals(object obj)
        {
            if (obj is EnemyNameAndRank other)
            {
                return EnemyName == other.EnemyName && Rank == other.Rank;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(EnemyName, Rank);
        }
    }
}
