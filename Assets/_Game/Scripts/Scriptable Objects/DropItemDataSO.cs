using _Game.Scripts.Helper;
using _Game.Scripts.Manager;
using _Game.Scripts.Scriptable_Objects.Database;
using _Game.Scripts.UI;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Scriptable_Objects
{
    [CreateAssetMenu(fileName = "Drop List", menuName = "DataBase/DropItem", order = 0)]
    public class DropItemDataSO : ScriptableObject
    {
        public Droppable PosibleDropItems;

        [System.Serializable]
        public class Droppable : UnitySerializedDictionary<ItemSO, AmountDrop>
        {
        }

        [System.Serializable]
        public class AmountDrop
        {
            public int amountMin;
            public int amountMax;
        }

        public async UniTask<List<ItemObject>> DropItemsByRate(ItemObject itemObjectPrefab, Vector3 position, int numberOfItems = 1)
        {
            List<ItemObject> droppedItems = new List<ItemObject>();
            List<ItemSO> validItems = new List<ItemSO>();
            int randomValue = Random.Range(1, 101);

            foreach (var entry in PosibleDropItems)
            {
                ItemSO item = entry.Key;
                int dropRate = item.ItemDropRate;
                if (dropRate < randomValue)
                {
                    validItems.Add(item);
                }
            }

            Dictionary<ItemSO, int> itemAmounts = new Dictionary<ItemSO, int>();

            if (validItems.Count > 0)
            {
                foreach (var item in validItems)
                {
                    for (int i = 0; i < numberOfItems; i++)
                    {
                        var offset = Random.insideUnitCircle * 0.5f;
                        var dropPosition = new Vector3(position.x + offset.x, position.y + offset.y, position.z);

                        var droppedItemGO = Instantiate(itemObjectPrefab, dropPosition, Quaternion.identity);
                        var droppedItem = droppedItemGO.GetComponent<ItemObject>();

                        droppedItem.ItemData = item;

                        droppedItem.LoadItem();

                        int amount = Random.Range(PosibleDropItems[item].amountMin, PosibleDropItems[item].amountMax);
                        itemAmounts[item] = amount;

                        droppedItem.SetAmount(amount);

                        droppedItems.Add(droppedItem);
                    }
                }
            }

            await UniTask.Delay(1000);

            foreach (var item in validItems)
            {
                int amount = itemAmounts[item];

                if (item.ItemID== 0)
                {
                    RankingManager.Instance.UserInformation.Beli += amount;
                    UserManagerUI.Instance.SaveUserInformation();
                    UserManagerUI.Instance.UpdateDisplayUser();
                }
                else if (item.ItemID == 1)
                {
                    RankingManager.Instance.UserInformation.Diamond += amount;
                    UserManagerUI.Instance.SaveUserInformation();
                    UserManagerUI.Instance.UpdateDisplayUser();
                }
            }
            return droppedItems;
        }
    }
}
