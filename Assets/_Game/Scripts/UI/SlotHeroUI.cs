using UnityEngine;
using UnityEngine.UI;
using TMPro;
using _Game.Scripts.Scriptable_Object;
using _Game.Scripts.Manager;
using System.Collections.Generic;

namespace _Game.Scripts.UI
{
    public class SlotHeroUI : MonoBehaviour
    {
        [SerializeField]
        private Image _avatarImage;

        [SerializeField]
        private TextMeshProUGUI _heroNameText;

        [SerializeField]
        private Button _selectedBtn;

        private HeroData _heroData;

        public bool Selected;

        private void Start()
        {
            _selectedBtn.onClick.RemoveAllListeners();
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
            _heroNameText.text = heroName;

            _heroData = heroData;
        }

        private void OnHeroSelected()
        {
            if (Selected)
            {
                RemoveHero();
            }
            else
            {
                AddHero();
            }
            Selected = !Selected;
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
                HeroesUI heroesUI = FindObjectOfType<HeroesUI>();
                heroesUI.DisplayReadyHeroes(heroList);
            }
        }

        private void RemoveHero()
        {
            var heroList = HeroManager.Instance.HeroesReady[0].heroes;
            heroList.Remove(_heroData);
            if (!heroList.Contains(_heroData))
            {
                heroList.Remove(_heroData);
                HeroesUI heroesUI = FindObjectOfType<HeroesUI>();
                heroesUI.DisplayReadyHeroes(heroList);
            }
        }
    }
}
