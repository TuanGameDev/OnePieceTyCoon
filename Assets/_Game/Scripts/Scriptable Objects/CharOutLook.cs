using _Game.Scripts.Helper;
using _Game.Scripts.Non_Mono;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace _Game.Scripts.Scriptable_Object
{
    [CreateAssetMenu(fileName = "CharOutLook", menuName = "_Game/Character/OutLook", order = 0)]
    public class CharOutLook : ScriptableObject
    {
        public CharOut CharOut;
    }

    [System.Serializable]
    public class CharOut : UnitySerializedDictionary<CharacterState, OutLook>
    {
    }


    [System.Serializable]
    public class OutLook
    {
        public GameObject Root;
    }
}
