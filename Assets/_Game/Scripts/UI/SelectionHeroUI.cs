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
    public class SelectionHeroUI : MonoBehaviour
    {
        [SerializeField]
        public CharOutLook _charOutLook;

        [SerializeField]
        public SlotHeroUI _slotHeroPrefab;

        [SerializeField]
        private Transform _heroesContainer;

        [SerializeField]
        private Button CountinueBtn;

        public TextMeshProUGUI MessageTxt;

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

        public void LoadAndDisplayHeroesReady()
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
              //  slotHero.SetHeroUI(hero.HeroAvatarPath, hero.HeroName, hero);
            }
        }
        public IEnumerator HideTxt(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            MessageTxt.text = "";
        }
    }
}
