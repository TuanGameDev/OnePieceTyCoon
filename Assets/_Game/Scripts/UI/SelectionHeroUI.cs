using _Game.Scripts.Character.Hero;
using _Game.Scripts.Manager;
using _Game.Scripts.Non_Mono;
using _Game.Scripts.Scriptable_Object;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI
{
    public class SelectionHeroUI : MonoBehaviour
    {
        [SerializeField]
        public CharOutLook _charOutLook;

        [SerializeField]
        private HeroController _heroPrefab;

        [SerializeField]
        private SlotHeroUI _slotHeroPrefab;

        [SerializeField]
        private Transform _heroesContainer;

        [SerializeField]
        private Transform _heroesReadyContainer;

        [SerializeField]
        private Button CountinueBtn;

        private const int MaxHeroesReady = 1;

        #region LoadDataHero

        public void LoadAndDisplayHeroes()
        {
            List<HeroData> availableHeroes = HeroManager.Instance.GetAvailableHeroes();

            if (availableHeroes.Count > 0)
            {
                DisplayHeroes(availableHeroes);
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
                slotHero.SetHeroUI(hero.HeroAvatarPath, hero.HeroName, hero);
            }
        }
    }
}
