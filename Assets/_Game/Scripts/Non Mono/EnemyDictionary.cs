using _Game.Scripts.Helper;
using _Game.Scripts.Non_Mono;
using _Game.Scripts.Scriptable_Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace _Game.Scripts.Non_Mono
{
    [System.Serializable]
    public class EnemyDictionary : UnitySerializedDictionary<EnemyNameAndRank, HeroDataSO>
    {

    }
}
