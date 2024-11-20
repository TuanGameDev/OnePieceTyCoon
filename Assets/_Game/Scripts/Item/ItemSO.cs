using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace _Game.Scripts.Scriptable_Objects.Database
{
    [CreateAssetMenu(fileName = "New Item", menuName = "_Game/Item/ItemSO", order = 0)]
    public class ItemSO : ScriptableObject
    {
        public Sprite ItemIcon;
        public int ItemID;
        public string ItemName;

        [TextArea]
        public string Description;

        [InfoBox("The bigger drop rate, the less it can be dropped")]
        public int ItemDropRate;
    }
}
