using _Game.Scripts.Enums;
using _Game.Scripts.Manager;
using _Game.Scripts.Non_Mono;
using _Game.Scripts.Scriptable_Object;
using _Game.Scripts.Scriptable_Objects.Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI
{
    public class SpinUI : MonoBehaviour
    {
        [SerializeField]
        private Transform _containerSpin;

        [SerializeField]
        private Transform _arrow;

        [SerializeField]
        private Button _spinBtn,_getSpinBtn;

        [SerializeField]
        private TextMeshProUGUI _messageTxt;

        [SerializeField]
        private float spinDuration = 3f;

        [SerializeField]
        private int extraRounds = 1;

        [SerializeField]
        private List<InfoItem> _infoItems;

        [SerializeField]
        private List<HeroData> _heroesData = new List<HeroData>();

        [SerializeField]
        private List<ItemData> _itemData = new List<ItemData>();

        private bool isSpinning = false;

        #region InfoHero

        [SerializeField]
        private ElementalImgDictionary _elementalImgDictionary;

        [SerializeField]
        private Image _avatarHero;

        [SerializeField]
        private Image _elementalHero;

        [SerializeField]
        private TextMeshProUGUI _nameHeroTxt;

        [SerializeField]
        private TextMeshProUGUI _rarityHeroTxt;

        [SerializeField]
        private GameObject _panelItemPopup;

        #endregion

        private void Start()
        {
            _spinBtn.onClick.AddListener(StartSpin);
            _getSpinBtn.onClick.AddListener(GetSpin);
        }

        private void StartSpin()
        {
            int totalAmount = 1000;

            if (RankingManager.Instance.UserInformation.Diamond < totalAmount)
            {
                _messageTxt.gameObject.SetActive(true);
                _messageTxt.color = Color.red;
                _messageTxt.text = "You don't have enough diamonds to spin!";
                StartCoroutine(HideTxt(1f));
            }
            else
            {
                if (!isSpinning)
                {
                    RankingManager.Instance.UserInformation.Diamond -= totalAmount;
                    StartCoroutine(Spin());
                }
            }
            UserManagerUI.Instance.SaveUserInformation();
            UserManagerUI.Instance.UpdateDisplayUser();
        }

        private void GetSpin()
        {
            if (_heroesData.Count == 0 && _itemData.Count == 0)
            {
                return;
            }
            foreach (var hero in _heroesData)
            {
                HeroManager.Instance.AddHero(hero);
            }

            foreach (var item in _itemData)
            {
                if (item.ItemID == 0)
                {
                    RankingManager.Instance.AddBeli(item.Quantity);
                }
                else if (item.ItemID == 1)
                {
                    RankingManager.Instance.AddDiamond(item.Quantity);
                }
            }

            _getSpinBtn.gameObject.SetActive(false);
            _spinBtn.gameObject.SetActive(true);
            _heroesData.Clear();
            _itemData.Clear();

            _messageTxt.gameObject.SetActive(true);
            _messageTxt.color = Color.green;
            _messageTxt.text = "Get success!";
            StartCoroutine(HideTxt(1f));

            UserManagerUI.Instance.SaveUserInformation();
            UserManagerUI.Instance.UpdateDisplayUser();
        }

        private IEnumerator Spin()
        {
            isSpinning = true;

            int randomChance = Random.Range(0, 101);
            Debug.Log($"Tỉ lệ Spin: {randomChance}%");

            List<InfoItem> filteredItems;

            if (randomChance < 1)
            {
                filteredItems = _infoItems.FindAll(item =>
                    item.SlotItemSpinUI != null &&
                    item.SlotItemSpinUI.StatItemType == StatItem.Hero &&
                    item.SlotItemSpinUI.HeroData.Rarity == Rarity.SS);
            }
            else if (randomChance < 10)
            {
                filteredItems = _infoItems.FindAll(item =>
                    item.SlotItemSpinUI != null &&
                    item.SlotItemSpinUI.StatItemType == StatItem.Hero &&
                    item.SlotItemSpinUI.HeroData.Rarity == Rarity.S);
            }
            else if (randomChance < 19)
            {
                filteredItems = _infoItems.FindAll(item =>
                    item.SlotItemSpinUI != null &&
                    item.SlotItemSpinUI.StatItemType == StatItem.Hero &&
                    item.SlotItemSpinUI.HeroData.Rarity == Rarity.A);
            }
            else
            {
                filteredItems = _infoItems.FindAll(item =>
                    item.SlotItemSpinUI != null &&
                    item.SlotItemSpinUI.StatItemType == StatItem.Item);
            }

            if (filteredItems.Count == 0)
            {
                Debug.LogWarning("No items found matching the criteria! Using full list.");
                filteredItems = _infoItems;
            }

            int selectedIndex = Random.Range(0, filteredItems.Count);
            InfoItem selectedItem = filteredItems[selectedIndex];

            int globalIndex = _infoItems.IndexOf(selectedItem);

            float anglePerSlot = 360f / _infoItems.Count;
            float targetAngle = -globalIndex * anglePerSlot;

            float currentAngle = _containerSpin.eulerAngles.z;
            float totalSpinAngle = targetAngle - currentAngle - (360f * extraRounds);

            if (totalSpinAngle > 0)
            {
                totalSpinAngle -= 360f;
            }

            totalSpinAngle -= 360f;

            float elapsedTime = 0f;
            float totalTime = spinDuration;
            float startAngle = currentAngle;

            while (elapsedTime < totalTime)
            {
                float t = elapsedTime / totalTime;
                float easedT = 1f - Mathf.Pow(1f - t, 3);

                float angle = Mathf.Lerp(startAngle, startAngle + totalSpinAngle, easedT);
                _containerSpin.eulerAngles = new Vector3(0, 0, angle);

                elapsedTime += Time.deltaTime;
                yield return null;
            }
            _containerSpin.eulerAngles = new Vector3(0, 0, targetAngle);

            ProcessSelectedItem(selectedItem);

            isSpinning = false;
            _spinBtn.gameObject.SetActive(false);
            _getSpinBtn.gameObject.SetActive(true);
            _messageTxt.gameObject.SetActive(true);
            _messageTxt.color = Color.green;
            _messageTxt.text = "Congratulations on receiving your item!";
            StartCoroutine(HideTxt(1f));

            if (selectedItem.SlotItemSpinUI != null && selectedItem.SlotItemSpinUI.StatItemType == StatItem.Hero)
            {
                _panelItemPopup.gameObject.SetActive(true);
                UpdateUIHero();
            }
        }

        private void UpdateUIHero()
        {
            if(_heroesData != null)
            {
                _avatarHero.sprite = _heroesData[0].HeroAvatar;
                _nameHeroTxt.text = _heroesData[0].CharacterName.ToString();
                _rarityHeroTxt.text = _heroesData[0].Rarity.ToString();

                if (_elementalImgDictionary != null && _heroesData != null)
                {
                    if (_elementalImgDictionary.TryGetValue(_heroesData[0].Elemental, out Sprite elementalSprite))
                    {
                        _elementalHero.sprite = elementalSprite;
                    }
                }
            }
        }

        private void ProcessSelectedItem(InfoItem selectedItem)
        {
            if (selectedItem == null || selectedItem.SlotItemSpinUI == null)
            {
                return;
            }

            SlotItemSpinUI slotItem = selectedItem.SlotItemSpinUI;

            switch (slotItem.StatItemType)
            {
                case StatItem.Hero:
                    if (slotItem.HeroData != null)
                    {
                        _heroesData.Add(slotItem.HeroData);
                        Debug.Log($"HeroData added: {slotItem.HeroData.CharacterName}");
                    }
                    break;

                case StatItem.Item:
                    if (slotItem.ItemData != null)
                    {
                        _itemData.Add(slotItem.ItemData);
                        Debug.Log($"ItemData added: {slotItem.ItemData.ItemName}");
                    }
                    break;

                default:
                    Debug.LogWarning("Unhandled StatItemType.");
                    break;
            }
        }
        private IEnumerator HideTxt(float delay)
        {
            yield return new WaitForSeconds(delay);
            _messageTxt.gameObject.SetActive(false);
        }
    }

    [System.Serializable]
    public class InfoItem
    {
        public SlotItemSpinUI SlotItemSpinUI;
    }
}
