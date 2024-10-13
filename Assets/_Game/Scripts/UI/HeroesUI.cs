using _Game.Scripts.Manager;
using _Game.Scripts.Scriptable_Object;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI
{
    public class HeroesUI : MonoBehaviour
    {
        [SerializeField]
        private SlotHeroUI _slotHeroPrefab;

        [SerializeField]
        private SlotHeroReadyUI _slotHeroReadyPrefab;

        [SerializeField]
        private Transform _heroesContainer;

        [SerializeField]
        private Transform _heroesReadyContainer;

        [SerializeField]
        private Button CountinueBtn;

        private const int MaxHeroesReady = 1;
        #region LoadDataHero
        public void LoadListHero()
        {
            LoadAndDisplayHeroes();
            LoadAndDisplayReadyHeroes();
        }
        private void LoadAndDisplayHeroes()
        {
            List<HeroData> availableHeroes = HeroManager.Instance.GetAvailableHeroes();

            if (availableHeroes.Count > 0)
            {
                DisplayHeroes(availableHeroes);
            }
        }

        private void LoadAndDisplayReadyHeroes()
        {
            List<HeroData> availableHeroes = HeroManager.Instance.GetAvailableHeroesReady();

            if (availableHeroes.Count > 0)
            {
                DisplayReadyHeroes(availableHeroes);
            }
        }

        #endregion
        public void UpdateStartGameButtonState()
        {
            if (HeroManager.Instance.HeroesReady[0].heroes.Count >= MaxHeroesReady)
            {
                CountinueBtn.interactable = true;
            }
            else
            {
                CountinueBtn.interactable = false;
            }
        }

        public void DisplayReadyHeroes(List<HeroData> heroesReady)
        {
            foreach (Transform child in _heroesReadyContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (var hero in heroesReady)
            {
                var slotHeroReady = Instantiate(_slotHeroReadyPrefab, _heroesReadyContainer);
                slotHeroReady.SetHeroUI(hero.HeroAvatar);
            }

            UpdateStartGameButtonState();
        }
        public void DisplayHeroes(List<HeroData> heroes)
        {
            foreach (Transform child in _heroesContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (var hero in heroes)
            {
                var slotHero = Instantiate(_slotHeroPrefab, _heroesContainer);
                slotHero.SetHeroUI(hero.IconPath, hero.HeroName, hero);
            }
        }
    }
}
