using UnityEngine;
using UnityEngine.UI;
using _Game.Scripts.Scriptable_Object;
using _Game.Scripts.Manager;
using System.Collections.Generic;
using _Game.Scripts.Character.Hero;

namespace _Game.Scripts.UI
{
    public class SlotHeroUI : MonoBehaviour
    {
        [SerializeField]
        private Image _avatarImage;

        [SerializeField]
        private Image _selectionImage;

        [SerializeField]
        private Button _selectedBtn;

        private HeroData _heroData;

        public bool Selected;

        private void Start()
        {
            _selectedBtn.onClick.AddListener(OnHeroSelected);
        }

        public void SetHeroUI(string avatarPath, string heroName, HeroData heroData)
        {
            if (!string.IsNullOrEmpty(avatarPath))
            {
                Sprite avatarSprite = Resources.Load<Sprite>(avatarPath);
                if (avatarSprite != null)
                {
                    _avatarImage.sprite = avatarSprite;
                }
            }

            _heroData = heroData;

            if (HeroManager.Instance.HeroesReady.Count > 0)
            {
                var heroesReadyList = HeroManager.Instance.HeroesReady[0].heroes;
                Selected = heroesReadyList.Contains(_heroData);
            }
            else
            {
                Selected = false;
            }

            UpdateSelectedButtonUI();
        }


        private void OnHeroSelected()
        {
            if (Selected && !HeroManager.Instance.HeroesReady[0].heroes.Contains(_heroData))
            {
                AddHero();
            }
            else if (Selected)
            {
                RemoveHero();
            }
            else
            {
                AddHero();
            }

            Selected = !Selected;
            UpdateSelectedButtonUI();
        }


        private void AddHero()
        {
            if (HeroManager.Instance.HeroesReady.Count == 0)
            {
                HeroManager.Instance.HeroesReady.Add(new HeroDataList { heroes = new List<HeroData>() });
            }

            var heroList = HeroManager.Instance.HeroesReady[0].heroes;

            if (!heroList.Contains(_heroData))
            {
                heroList.Add(_heroData);

                // Check if we have a valid spawn index before calling GetHeroes
                int spawnIndex = HeroManager.Instance.GetAvailableSpawnIndex();
                if (spawnIndex != -1)
                {
                    HeroManager.Instance.GetHeroes(); // Spawn hero immediately
                }
                else
                {
                    Debug.Log("No available spawn points when trying to add a hero.");
                }

                SelectionHeroUI heroesUI = FindObjectOfType<SelectionHeroUI>();
                heroesUI.DisplayReadyHeroes(heroList);
            }
        }

        private void RemoveHero()
        {
            var heroList = HeroManager.Instance.HeroesReady[0].heroes;
            heroList.Remove(_heroData);

            if (!heroList.Contains(_heroData))
            {
                HeroManager.Instance.RemoveSpawnedHero(_heroData);

                SelectionHeroUI heroesUI = FindObjectOfType<SelectionHeroUI>();
                heroesUI.DisplayReadyHeroes(heroList);
            }
        }
        private void UpdateSelectedButtonUI()
        {
            _selectionImage.GetComponent<Image>().enabled = Selected;
        }
    }
}
