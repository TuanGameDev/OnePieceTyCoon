using _Game.Scripts.Scriptable_Object;
using _Game.Scripts.Scriptable_Objects.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI
{
    public enum StatItem
    {
        None,
        Hero,
        Item,
        Weapon,
    }
    public class SlotItemSpinUI : MonoBehaviour
    {
        [SerializeField]
        private Image _avatarItem;

        [SerializeField]
        private TextMeshProUGUI _nameItemTxt;

        [SerializeField]
        private TextMeshProUGUI _amountItemTxt;


        public HeroData HeroData;
        public ItemData ItemData;

        public StatItem StatItemType;

        private void OnEnable()
        {
            UpdateUI();
        }

        public void SetStatItem(SlotItemSpinUI slotItemSpinUI)
        {
            if (slotItemSpinUI == null) return;

            HeroData = slotItemSpinUI.HeroData;
            ItemData = slotItemSpinUI.ItemData;
            StatItemType = slotItemSpinUI.StatItemType;

            UpdateUI();
        }

        private void UpdateUI()
        {
            if(_avatarItem !=null && _nameItemTxt!=null && _amountItemTxt !=null)
            {
                switch (StatItemType)
                {
                    case StatItem.Hero:
                        if (HeroData != null)
                        {
                            _avatarItem.sprite = HeroData.IconAvatar;
                            _nameItemTxt.text = HeroData.CharacterName.ToString();
                            _amountItemTxt.text = $"{HeroData.Rarity}";
                        }
                        break;

                    case StatItem.Item:
                        if (ItemData != null)
                        {
                            _avatarItem.sprite = ItemData.ItemIcon;
                            _nameItemTxt.text = ItemData.ItemName;
                            _amountItemTxt.text = $"{ItemData.Quantity}";
                        }
                        break;

                    default:
                        _avatarItem.sprite = null;
                        _nameItemTxt.text = "No Data";
                        _amountItemTxt.text = "";
                        break;
                }
            }
        }
    }
}