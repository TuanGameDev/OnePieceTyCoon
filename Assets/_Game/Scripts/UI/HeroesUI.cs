using _Game.Scripts.Manager;
using _Game.Scripts.Scriptable_Object;
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

        public GameUI GameUI;

        private const int MaxHeroesReady = 1;

        public void UpdateStartGameButtonState()
        {
            if (HeroManager.Instance.HeroesReady[0].heroes.Count >= MaxHeroesReady)
            {
                GameUI.StartGameBtn.interactable = true;
            }
            else
            {
                GameUI.StartGameBtn.interactable = false;
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

        public void AddOrRemoveHeroFromReady(HeroData hero)
        {
            var heroesReadyList = HeroManager.Instance.HeroesReady[0].heroes;
            if (heroesReadyList.Contains(hero))
            {
                heroesReadyList.Remove(hero);
            }
            else if (heroesReadyList.Count < MaxHeroesReady)
            {
                heroesReadyList.Add(hero);
            }
            DisplayReadyHeroes(heroesReadyList);
        }
    }
}
