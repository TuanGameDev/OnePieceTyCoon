using _Game.Scripts.Character.Hero;
using _Game.Scripts.Manager;
using _Game.Scripts.Scriptable_Object;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using _Game.Scripts.Helper;
using _Game.Scripts.Enums;

namespace _Game.Scripts.UI
{
    public class HeroesUI : Singleton<HeroesUI>, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] 
        private RarityAndColorDictionary _rarityAndColorDictionary;

        [SerializeField] 
        private SlotHeroUI _slotHeroPrefab;

        [SerializeField] 
        private Transform _heroesContainer;

        [SerializeField]
        private Image _heroIconAvatar,_iconLock;

        [SerializeField] 
        private TextMeshProUGUI _nameHeroTxt, _rarityHeroTxt, _powerHeroTxt, _levelHeroTxt, _stateHeroTxt, _heroSlotTxt;

        [SerializeField] 
        private Button _infoHeroBtn, _statHeroBtn, _removeHeroBtn;

        [SerializeField] 
        private GameObject _statHeroPopup, _infoHeroStatPopup;

        private List<SlotHeroUI> _heroSlots = new List<SlotHeroUI>();

        private Dictionary<string, bool> heroCombatStatus = new Dictionary<string, bool>();

        private SlotHeroUI _selectedHero;

        private void Start()
        {
            _statHeroBtn.onClick.AddListener(OnStatHeroButtonClicked);
            _removeHeroBtn.onClick.AddListener(() => RemoveHero(_selectedHero));
            HeroManager.Instance.OnAddHero += LoadAndDisplayHeroes;
            _statHeroPopup.SetActive(false);
            LoadHeroCombatStatus();
        }

        private void OnDestroy() => HeroManager.Instance.OnAddHero -= LoadAndDisplayHeroes;

        private void SaveHeroCombatStatus()
        {
            foreach (var slotHero in _heroSlots)
            {
                string key = GetHeroCombatKey(slotHero.HeroData.HeroID, _heroSlots.IndexOf(slotHero));
                heroCombatStatus[key] = slotHero.IsInCombat;
            }
        }

        private void LoadHeroCombatStatus()
        {
            foreach (var slotHero in _heroSlots)
            {
                string key = GetHeroCombatKey(slotHero.HeroData.HeroID, _heroSlots.IndexOf(slotHero));
                slotHero.IsInCombat = heroCombatStatus.ContainsKey(key) ? heroCombatStatus[key] : false;
            }
        }

        private string GetHeroCombatKey(int heroID, int slotIndex) => $"{heroID}_{slotIndex}";

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.pointerPress == _infoHeroBtn.gameObject) _infoHeroStatPopup.SetActive(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerPress == _infoHeroBtn.gameObject) _infoHeroStatPopup.SetActive(false);
        }

        public void SelectHero(SlotHeroUI selectedHero)
        {
            _selectedHero = selectedHero;
            foreach (var slot in _heroSlots)
                slot.SetSelected(slot == selectedHero);

            _statHeroPopup.SetActive(_selectedHero != null);
            UpdateStatHero();

            UpdateStatHeroButtonState(selectedHero);

            UpdateStateText();
        }

        private void UpdateStatHeroButtonState(SlotHeroUI selectedHero)
        {
            int heroID = selectedHero.HeroData.HeroID;
            bool isAnyHeroInCombat = false;
            _iconLock.gameObject.SetActive(false);
            foreach (var slot in _heroSlots)
            {
                if (slot.HeroData.HeroID == heroID && slot.IsInCombat && slot != selectedHero)
                {
                    isAnyHeroInCombat = true;
                    _iconLock.gameObject.SetActive(true);
                    break;
                }
            }
            _statHeroBtn.interactable = !isAnyHeroInCombat;
        }

        private void UpdateStatHero()
        {
            if (_selectedHero == null) return;

            _nameHeroTxt.text = _selectedHero.HeroData.CharacterName.ToString();
            _rarityHeroTxt.text = _selectedHero.HeroData.Rarity.ToString();
            _rarityHeroTxt.color = _rarityAndColorDictionary.TryGetValue(_selectedHero.HeroData.Rarity, out Color color) ? color : Color.white;
            _powerHeroTxt.text = $"Power: {_selectedHero.HeroData.Power}";
            _levelHeroTxt.text = $"Lv. {_selectedHero.HeroData.CharacterStat.HeroLevel}";

            _heroIconAvatar.sprite = Resources.Load<Sprite>(_selectedHero.HeroData.HeroAvatarPath);
        }

        private void OnStatHeroButtonClicked()
        {
            if (_selectedHero != null)
            {
                ToggleHeroState(_selectedHero);
                UpdateStateText();
            }
        }

        private void RemoveHero(SlotHeroUI heroSlot)
        {
            if (heroSlot == null) return;

            int heroID = heroSlot.HeroData.HeroID;
            HeroManager.Instance.GetAvailableHeroes().RemoveAll(hero => hero.HeroID == heroID);
            HeroManager.Instance.SaveDataHero();
            UserManagerUI.Instance.RecalculateCombatPower();

            _heroSlots.Remove(heroSlot);
            Destroy(heroSlot.gameObject);

            if (_selectedHero == heroSlot)
            {
                _selectedHero = null;
                _statHeroPopup.SetActive(false);
            }

            SpawnHeroManager.Instance.RemoveHero(heroID);
        }

        private void ToggleHeroState(SlotHeroUI selectedHero)
        {
            int heroID = selectedHero.HeroData.HeroID;
            int slotIndex = _heroSlots.IndexOf(selectedHero);
            string key = GetHeroCombatKey(heroID, slotIndex);

            HeroController heroInstance = SpawnHeroManager.Instance.GetSpawnedHero(heroID);

            if (heroInstance != null)
            {
                heroCombatStatus[key] = false;
                selectedHero.IsInCombat = false;
                SpawnHeroManager.Instance.RemoveHero(heroID);
                HeroControllerUI.Instance.UpdateDisplayHeroes();
            }
            else
            {
                heroCombatStatus[key] = true;
                selectedHero.IsInCombat = true;
                SpawnHeroManager.Instance.SpawnHero(selectedHero.HeroData);
                HeroControllerUI.Instance.UpdateDisplayHeroes();
            }
        }

        public void UpdateHeroSlotText()
        {
            if (HeroManager.Instance == null)
            {
                Debug.LogError("HeroManager.Instance is null");
                return;
            }

            int currentHeroCount = HeroManager.Instance.GetAvailableHeroes()?.Count ?? 0;
            int maxHeroSlot = HeroManager.Instance.MaxHeroSlot;
            if (_heroSlotTxt != null)
            {
                _heroSlotTxt.text = $"{currentHeroCount}/{maxHeroSlot}";
            }
        }


        private void UpdateStateText()
        {
            if (_selectedHero != null && _selectedHero.IsInCombat)
            {
                _stateHeroTxt.text = "Withdrew";
                _stateHeroTxt.color = Color.red;
            }
            else
            {
                _stateHeroTxt.text = "Combat";
                _stateHeroTxt.color = Color.yellow;
            }
        }

        public void LoadAndDisplayHeroes()
        {
            List<HeroData> availableHeroes = HeroManager.Instance.GetAvailableHeroes();
            if (availableHeroes.Count > 0) DisplayHeroes(availableHeroes);
        }

        public void DisplayHeroes(List<HeroData> heroes)
        {
            foreach (Transform child in _heroesContainer) Destroy(child.gameObject);
            _heroSlots.Clear();

            foreach (var hero in heroes)
            {
                var slotHero = Instantiate(_slotHeroPrefab, _heroesContainer);
                slotHero.SetHeroUI(hero.IconAvatarPath, hero, this);

                string key = GetHeroCombatKey(hero.HeroID, _heroSlots.Count);
                slotHero.IsInCombat = heroCombatStatus.ContainsKey(key) ? heroCombatStatus[key] : false;

                _heroSlots.Add(slotHero);
            }
        }

        private void OnApplicationQuit() => SaveHeroCombatStatus();
    }

    [System.Serializable]
    public class RarityAndColorDictionary : UnitySerializedDictionary<Rarity, Color> { }
}
