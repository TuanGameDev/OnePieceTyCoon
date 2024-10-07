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

        public void DisplayHeroes(List<HeroData> heroes)
        {
            foreach (Transform child in _heroesContainer)
            {
                Destroy(child.gameObject);
            }
            foreach (var hero in heroes)
            {
                var slotHero = Instantiate(_slotHeroPrefab, _heroesContainer);
                slotHero.SetHeroUI(hero.AvatarPath, hero.HeroName,hero);
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
        }
    }
}
