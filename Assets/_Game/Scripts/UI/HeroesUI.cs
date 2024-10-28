using _Game.Scripts.Character.Hero;
using _Game.Scripts.Manager;
using _Game.Scripts.Non_Mono;
using _Game.Scripts.Scriptable_Object;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI
{
    public class HeroesUI : MonoBehaviour
    {
        [SerializeField]
        public SlotHeroUI _slotHeroPrefab;

        [SerializeField]
        private Transform _heroesContainer;

        private void Start()
        {
            LoadAndDisplayHeroes();
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

            foreach (var hero in heroes)
            {
                var slotHero = Instantiate(_slotHeroPrefab, _heroesContainer);
                slotHero.SetHeroUI(hero.HeroAvatarPath, hero);
            }
        }
    }
}
