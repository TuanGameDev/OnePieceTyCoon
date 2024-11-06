using _Game.Scripts.Character.Hero;
using _Game.Scripts.Manager;
using _Game.Scripts.Scriptable_Object;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using _Game.Scripts.Helper;

namespace _Game.Scripts.UI
{
    public class HeroesUI : Singleton<HeroesUI>
    {
        [SerializeField] 
        private SlotHeroUI _slotHeroPrefab;

        [SerializeField] 
        private Transform _heroesContainer;

        [SerializeField] 
        private Image _heroIconAvatar;

        [SerializeField] 
        private TextMeshProUGUI _nameHeroTxt;

        [SerializeField] 
        private TextMeshProUGUI _stateHeroTxt;

        [SerializeField] 
        private Button _infoHeroBtn;

        [SerializeField] 
        private Button _statHeroBtn;

        [SerializeField] 
        private Button _removeHeroBtn;

        [Header("Popup")]
        [SerializeField]
        private GameObject _statHeroPopup;

        [SerializeField]
        private GameObject _infoHeroPopup;

        private List<SlotHeroUI> _heroSlots = new List<SlotHeroUI>();
        private SlotHeroUI _selectedHero;

        private void Start()
        {
            Invoke(nameof(LoadAndDisplayHeroes), 1f);
            _statHeroBtn.onClick.AddListener(OnStatHeroButtonClicked);
            _removeHeroBtn.onClick.AddListener(() => RemoveHero(_selectedHero));
            HeroManager.Instance.OnAddHero += LoadAndDisplayHeroes;
            _statHeroPopup.SetActive(false);
        }

        private void OnDestroy()
        {
            HeroManager.Instance.OnAddHero -= LoadAndDisplayHeroes;
        }

        public void SelectHero(SlotHeroUI selectedHero)
        {
            _selectedHero = selectedHero;

            foreach (var slot in _heroSlots)
            {
                slot.SetSelected(slot == selectedHero);
            }
            _statHeroPopup.SetActive(_selectedHero != null);

            UpdateStatHero();
            UpdateStateText();
        }

        public void UpdateStatHero()
        {
            if (_selectedHero == null) return;

            _nameHeroTxt.text = _selectedHero.HeroData.CharacterName.ToString();
            Sprite avatarSprite = Resources.Load<Sprite>(_selectedHero.HeroData.HeroAvatarPath);
            if (avatarSprite != null)
            {
                _heroIconAvatar.sprite = avatarSprite;
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

        private void RemoveHero(SlotHeroUI heroSlot)
        {
            if (heroSlot == null) return;

            int heroID = heroSlot.HeroData.HeroID;

            var heroList = HeroManager.Instance.GetAvailableHeroes();
            HeroData heroToRemove = heroList.Find(hero => hero.HeroID == heroID);

            if (heroToRemove != null)
            {
                heroList.Remove(heroToRemove);

                HeroManager.Instance.SaveDataHero();

                UserManagerUI.Instance.RecalculateCombatPower();

                _heroSlots.Remove(heroSlot);
                Destroy(heroSlot.gameObject);

                if (_selectedHero == heroSlot)
                {
                    _selectedHero = null;
                    _statHeroPopup.SetActive(false);
                }
            }
            SpawnHeroManager.Instance.RemoveHero(heroID);
        }


        private void ToggleHeroState(SlotHeroUI selectedHero)
        {
            int heroID = selectedHero.HeroData.HeroID;
            HeroController heroInstance = SpawnHeroManager.Instance.GetSpawnedHero(heroID);

            if (heroInstance != null)
            {
                SpawnHeroManager.Instance.RemoveHero(heroID);
                selectedHero.IsInCombat = false;
            }
            else
            {
                SpawnHeroManager.Instance.SpawnHero(selectedHero.HeroData);
                selectedHero.IsInCombat = true;
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
            if (availableHeroes.Count > 0)
            {
                DisplayHeroes(availableHeroes);
            }
        }

        public void DisplayHeroes(List<HeroData> heroes)
        {
            foreach (Transform child in _heroesContainer)
            {
                Destroy(child.gameObject);
            }
            _heroSlots.Clear();

            foreach (var hero in heroes)
            {
                var slotHero = Instantiate(_slotHeroPrefab, _heroesContainer);
                slotHero.SetHeroUI(hero.IconAvatarPath, hero, this);
                slotHero.IsInCombat = false;
                _heroSlots.Add(slotHero);
            }
        }
    }
}
