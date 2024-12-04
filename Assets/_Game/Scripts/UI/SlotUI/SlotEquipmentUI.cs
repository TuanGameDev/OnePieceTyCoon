using _Game.Scripts.Character.Hero;
using _Game.Scripts.Characters;
using _Game.Scripts.Enums;
using _Game.Scripts.Scriptable_Object;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI
{
    public class SlotEquipmentUI : MonoBehaviour
    {
        [SerializeField]
        private RarityAndColorDictionary _rarityAndColorDictionary;

        [SerializeField]
        private Image _avatarEquiment;

        [SerializeField]
        private TextMeshProUGUI _levelEquipmentTxt;

        [SerializeField]
        private TextMeshProUGUI _rarityEquipmentTxt;

        [SerializeField]
        private Button _levelUpBtn;

        public CharacterStat BonusStatEquip;

        public EquipmentType EquipmentType;

        private System.Action<SlotEquipmentUI> _onLevelUpCallback;

        private void Awake()
        {
            _levelUpBtn.onClick.AddListener(() =>
            {
                _onLevelUpCallback?.Invoke(this);
            });
        }

        public void SetIcon(HeroData hero,Sprite icon, System.Action<SlotEquipmentUI> onLevelUpCallback,int level,string rarity)
        {
            _avatarEquiment.sprite = icon;
            _avatarEquiment.gameObject.SetActive(icon != null);

            _onLevelUpCallback = onLevelUpCallback;
            _levelEquipmentTxt.text = $"Lv. {level}";
            _rarityEquipmentTxt.text = $"{rarity}";
            _rarityEquipmentTxt.color = _rarityAndColorDictionary.TryGetValue(hero.Rarity, out Color color) ? color : Color.white;

        }

        public void SetLevelUpButtonVisible(bool isVisible)
        {
            _levelUpBtn.gameObject.SetActive(isVisible);
        }
    }
}
