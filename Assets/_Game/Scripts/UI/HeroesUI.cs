using _Game.Scripts.Character.Hero;
using _Game.Scripts.Manager;
using _Game.Scripts.Scriptable_Object;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI
{
    public class HeroesUI : MonoBehaviour
    {
        [SerializeField] private SlotHeroUI _slotHeroPrefab;

        [SerializeField] private Transform _heroesContainer;

        [SerializeField] private TextMeshProUGUI _nameHeroTxt;

        [SerializeField] private TextMeshProUGUI _stateHeroTxt;

        [SerializeField] private Button _statHeroBtn;

        [SerializeField] private Button _removeHeroBtn;

        private List<SlotHeroUI> _heroSlots = new List<SlotHeroUI>();

        private SlotHeroUI _selectedHero;

        private void Start()
        {
            Invoke(nameof(LoadAndDisplayHeroes), 1f);
            _statHeroBtn.onClick.AddListener(OnStatHeroButtonClicked);
            _removeHeroBtn.onClick.AddListener(() => RemoveHero(_selectedHero));
        }

        public void SelectHero(SlotHeroUI selectedHero)
        {
            _selectedHero = selectedHero;

            foreach (var slot in _heroSlots)
            {
                slot.SetSelected(slot == selectedHero);
            }

            UpdateStatHero(_selectedHero.HeroData.HeroName);
            UpdateStateText();
        }

        public void UpdateStatHero(string heroName)
        {
            _nameHeroTxt.text = heroName;
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
            heroList.RemoveAll(hero => hero.HeroID == heroID);
            HeroManager.Instance.SaveDataHero();

            SpawnHeroManager.Instance.RemoveHero(heroID);
            _heroSlots.Remove(heroSlot);
            Destroy(heroSlot.gameObject);
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
            _stateHeroTxt.text = _selectedHero != null && _selectedHero.IsInCombat ? "Withdrew" : "Combat";
        }

        private void LoadAndDisplayHeroes()
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
                slotHero.SetHeroUI(hero.HeroAvatarPath, hero, this);
                slotHero.IsInCombat = false;
                _heroSlots.Add(slotHero);
            }
        }
    }
}
