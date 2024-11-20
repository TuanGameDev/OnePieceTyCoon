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
        private Image _expBar;

        [SerializeField]
        private TextMeshProUGUI _nameHeroTxt, _rarityHeroTxt, _powerHeroTxt, _levelHeroTxt, _stateHeroTxt, _heroSlotTxt, _heroHpTxt, _heroDefTxt, _heroAttackTxt, _heroMoveTxt, _heroEXPTxt;

        [Space(20)]
        [SerializeField] 
        private Button _infoHeroBtn, _statHeroBtn,_upGradeBtn;

        public Button RemoveHeroBtn;

        [Space(20)]
        [SerializeField] 
        private GameObject _statHeroPopup, _infoHeroStatPopup, _expPopup, _upGradePopup;

        private List<SlotHeroUI> _heroSlots = new List<SlotHeroUI>();

        private Dictionary<string, bool> heroCombatStatus = new Dictionary<string, bool>();

        private SlotHeroUI _selectedHero;

        public void Start()
        {
            if (_statHeroBtn != null)
                _statHeroBtn.onClick.AddListener(OnStatHeroButtonClicked);

            if (RemoveHeroBtn != null)
                RemoveHeroBtn.onClick.AddListener(() => RemoveHero(_selectedHero));

            if (_upGradeBtn != null)
                _upGradeBtn.onClick.AddListener(LevelUpBtnClick);

            if (HeroManager.Instance != null)
                HeroManager.Instance.OnAddHero += LoadAndDisplayHeroes;

            if (_statHeroPopup != null)
                _statHeroPopup.SetActive(false);

            LoadHeroCombatStatus();
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

        private void OnDestroy() => HeroManager.Instance.OnAddHero -= LoadAndDisplayHeroes;

        private void LevelUpBtnClick()
        {
            if (_selectedHero == null) return;

            if (_selectedHero.HeroData.CharacterStat.HeroLevel >= 10)
            {
                _heroEXPTxt.text = "Max Level";
                _upGradePopup.SetActive(false);
                _expPopup.SetActive(true);
                return;
            }

            if (_selectedHero.HeroData.CharacterStat.CurrentExp >= _selectedHero.HeroData.CharacterStat.ExpToLevelUp)
            {
                _selectedHero.HeroData.CharacterStat.HeroLevel++;

                foreach (var levelStat in _selectedHero.HeroData.LevelStats)
                {
                    if (levelStat.StatLevel.HeroLevel == _selectedHero.HeroData.CharacterStat.HeroLevel)
                    {
                        _selectedHero.HeroData.CharacterStat = levelStat.StatLevel.Clone();

                        var spawnedHero = SpawnHeroManager.Instance.GetSpawnedHero(_selectedHero.HeroData.HeroID);
                        if (spawnedHero != null)
                        {
                            spawnedHero.HeroDataSO.CharacterStat = levelStat.StatLevel.Clone();
                            spawnedHero.CurrentStat = spawnedHero.HeroDataSO.CharacterStat;
                            _selectedHero.HeroData.CharacterStat = spawnedHero.CurrentStat;
                        }
                        break;
                    }
                }
                _expPopup.SetActive(true);
                _upGradePopup.SetActive(false);
                _infoHeroStatPopup.SetActive(true);
                HeroManager.Instance.SaveDataHero();
                UpdateStatHero();
            }
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
            if (selectedHero == null) return;

            int selectedSlotIndex = _heroSlots.IndexOf(selectedHero);
            if (selectedSlotIndex < 0) return;

            bool isHeroInCombat = false;
            _iconLock.gameObject.SetActive(false);

            foreach (var slot in _heroSlots)
            {
                if (slot != selectedHero && slot.HeroData.HeroID == selectedHero.HeroData.HeroID && slot.IsInCombat)
                {
                    isHeroInCombat = true;
                    _iconLock.gameObject.SetActive(true);
                    break;
                }
            }
            _statHeroBtn.interactable = !isHeroInCombat;
        }
        private void UpdateStatHero()
        {
            if (_selectedHero == null) return;

            if (_selectedHero.HeroData.CharacterStat.HeroLevel >= 10)
            {
                _heroEXPTxt.text = "Max Level";
                _expBar.fillAmount = 1f;
                _upGradePopup.SetActive(false);
                _expPopup.SetActive(true);
            }
            else
            {
                _heroEXPTxt.text = $"{_selectedHero.HeroData.CharacterStat.CurrentExp}/{_selectedHero.HeroData.CharacterStat.ExpToLevelUp}";
                _expBar.fillAmount = (float)_selectedHero.HeroData.CharacterStat.CurrentExp / _selectedHero.HeroData.CharacterStat.ExpToLevelUp;
            }

            _nameHeroTxt.text = _selectedHero.HeroData.CharacterName.ToString();
            _heroHpTxt.text = _selectedHero.HeroData.CharacterStat.Hp.ToString("N0");
            _heroDefTxt.text = _selectedHero.HeroData.CharacterStat.Def.ToString("N0");
            _heroAttackTxt.text = _selectedHero.HeroData.CharacterStat.AttackDamage.ToString("N0");
            _heroMoveTxt.text = _selectedHero.HeroData.CharacterStat.MoveSpeed.ToString();
            _rarityHeroTxt.text = _selectedHero.HeroData.Rarity.ToString();
            _rarityHeroTxt.color = _rarityAndColorDictionary.TryGetValue(_selectedHero.HeroData.Rarity, out Color color) ? color : Color.white;
            _powerHeroTxt.text = _selectedHero.HeroData.Power.ToString("N0");
            _levelHeroTxt.text = $"Lv. {_selectedHero.HeroData.CharacterStat.HeroLevel}";
            _heroIconAvatar.sprite = Resources.Load<Sprite>(_selectedHero.HeroData.HeroAvatarPath);

            if (_selectedHero.HeroData.CharacterStat.CurrentExp >= _selectedHero.HeroData.CharacterStat.ExpToLevelUp && _selectedHero.HeroData.CharacterStat.HeroLevel < 100)
            {
                _upGradePopup.SetActive(true);
                _expPopup.SetActive(false);
            }
            else
            {
                _upGradePopup.SetActive(false);
                _expPopup.SetActive(true);
            }
        }


        private void OnStatHeroButtonClicked()
        {
            if (_selectedHero != null)
            {
                ToggleHeroState(_selectedHero);
                UpdateStateText();
            }
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

        private void RemoveHero(SlotHeroUI heroSlot)
        {
            if (heroSlot == null) return;

            int slotIndex = _heroSlots.IndexOf(heroSlot);
            if (slotIndex < 0) return;

            int heroID = heroSlot.HeroData.HeroID;
            string key = GetHeroCombatKey(heroID, slotIndex);

            _heroSlots.Remove(heroSlot);
            Destroy(heroSlot.gameObject);

            var availableHeroes = HeroManager.Instance.GetAvailableHeroes();
            if (slotIndex >= 0 && slotIndex < availableHeroes.Count)
            {
                availableHeroes.RemoveAt(slotIndex);
            }

            HeroManager.Instance.SaveDataHero();
            UserManagerUI.Instance.RecalculateCombatPower();

            if (_selectedHero == heroSlot)
            {
                _selectedHero = null;
                _statHeroPopup.SetActive(false);
            }

            SpawnHeroManager.Instance.RemoveHero(heroID);

            LoadAndDisplayHeroes();
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
                _stateHeroTxt.color = Color.black;
            }
        }

        public void LoadAndDisplayHeroes()
        {
            var availableHeroes = HeroManager.Instance?.GetAvailableHeroes();
            if (availableHeroes == null || availableHeroes.Count == 0)
                return;

            var heroInCombatTracker = new HashSet<int>();

            ClearAllHeroSlots();
            _heroSlots.Clear();

            foreach (var hero in availableHeroes)
            {
                if (hero == null)
                    continue;

                if (_slotHeroPrefab == null)
                {
                    return;
                }

                if (_heroesContainer == null)
                {
                    return;
                }

                var slotHero = Instantiate(_slotHeroPrefab, _heroesContainer);
                if (slotHero == null)
                {
                    continue;
                }

                slotHero.SetHeroUI(hero.IconAvatarPath ?? string.Empty, hero, this);

                if (!heroInCombatTracker.Contains(hero.HeroID))
                {
                    slotHero.IsInCombat = heroCombatStatus.TryGetValue(GetHeroCombatKey(hero.HeroID, _heroSlots.Count), out bool isInCombat) && isInCombat;
                    if (slotHero.IsInCombat)
                    {
                        heroInCombatTracker.Add(hero.HeroID);
                    }
                }
                else
                {
                    slotHero.IsInCombat = false;
                }

                _heroSlots.Add(slotHero);
            }
        }


        public void DisplayHeroes(List<HeroData> heroes)
        {
            if (heroes == null || heroes.Count == 0)
                return;

            var heroInCombatTracker = new HashSet<int>();

            var heroCombatStatusBackup = new Dictionary<int, bool>();
            foreach (var slot in _heroSlots)
            {
                if (slot != null && slot.HeroData != null)
                {
                    heroCombatStatusBackup[slot.HeroData.HeroID] = slot.IsInCombat;
                }
            }

            ClearAllHeroSlots();
            _heroSlots.Clear();

            foreach (var hero in heroes)
            {
                if (hero == null)
                    continue;

                bool isInCombat = false;

                if (HeroControllerUI.Instance != null && HeroControllerUI.Instance.HeroCtrlUISlot != null)
                {
                    foreach (var heroControllerPair in HeroControllerUI.Instance.HeroCtrlUISlot)
                    {
                        var heroController = heroControllerPair.Value;

                        if (heroController.HeroDataSO != null && heroController.HeroDataSO.HeroID == hero.HeroID)
                        {
                            isInCombat = heroController.IsInCombat;
                            break;
                        }
                    }
                }

                if (!isInCombat && heroCombatStatusBackup.ContainsKey(hero.HeroID))
                {
                    isInCombat = heroCombatStatusBackup[hero.HeroID];
                }

 
                if (heroInCombatTracker.Contains(hero.HeroID))
                {
                    isInCombat = false;
                }
                else if (isInCombat)
                {
                    heroInCombatTracker.Add(hero.HeroID);
                }

                var slotHero = Instantiate(_slotHeroPrefab, _heroesContainer);
                if (slotHero == null)
                    continue;

                slotHero.SetHeroUI(hero.IconAvatarPath ?? string.Empty, hero, this);
                slotHero.IsInCombat = isInCombat;

                _heroSlots.Add(slotHero);
            }
        }

        public void ClearAllHeroSlots()
        {
            if (_heroesContainer == null || _heroSlots == null)
            {
                return;
            }

            foreach (Transform child in _heroesContainer)
            {
                Destroy(child.gameObject);
            }
            _heroSlots.Clear();
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.pointerPress == _infoHeroBtn.gameObject) _infoHeroStatPopup.SetActive(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerPress == _infoHeroBtn.gameObject) _infoHeroStatPopup.SetActive(false);
        }
    }

    [System.Serializable]
    public class RarityAndColorDictionary : UnitySerializedDictionary<Rarity, Color> { }
}
