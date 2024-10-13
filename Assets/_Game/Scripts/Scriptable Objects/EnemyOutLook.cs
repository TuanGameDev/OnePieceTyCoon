using _Game.Scripts.Helper;
using _Game.Scripts.Non_Mono;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace _Game.Scripts.Scriptable_Object.Enemy
{
    [CreateAssetMenu(fileName = "EnemyOutLook", menuName = "_Game/Enemy/OutLook", order = 0)]
    public class EnemyOutLook : ScriptableObject
    {
        public EnemyOut EnemyOut;
    }
    [System.Serializable]
    public class EnemyOut : UnitySerializedDictionary<EnemyNameAndRank, OutLook>
    {

    }
    [System.Serializable]
    public class OutLook
    {
        public GameObject Root;
    }
}
