using UnityEngine;
using UnityEngine.UI;
using _Game.Scripts.Scriptable_Object;
using _Game.Scripts.Helper;
using _Game.Scripts.Enums;
using _Game.Scripts.Non_Mono;

namespace _Game.Scripts.UI
{
    public class SlotHeroUI : MonoBehaviour
    {
        [SerializeField]
        private ElementalImgDictionary _elementalImgDictionary;

        [SerializeField]
        private Image[] _starImg;

        [SerializeField]  
        private Image _avatarHero;

        [SerializeField]
        private Image _elementalHero;

        [SerializeField] 
        private Image _selectedImg;

        [SerializeField] 
        private Button _selectedBtn;

        public HeroData HeroData;

        public bool IsInCombat;

        public void SetHeroUI(string avatarPath, HeroData heroData, HeroesUI heroesUI)
        {
            if (!string.IsNullOrEmpty(avatarPath))
            {
                Sprite avatarSprite = Resources.Load<Sprite>(avatarPath);
                if (avatarSprite != null)
                {
                    _avatarHero.sprite = avatarSprite;
                }
            }

            HeroData = heroData;

            if (_elementalImgDictionary != null && HeroData != null)
            {
                if (_elementalImgDictionary.TryGetValue(HeroData.Elemental, out Sprite elementalSprite))
                {
                    _elementalHero.sprite = elementalSprite;
                    _elementalHero.gameObject.SetActive(true);
                }
            }

            UpdateStarImages(HeroData.Rarity);

            _selectedImg.gameObject.SetActive(false);

            _selectedBtn.onClick.AddListener(() => heroesUI.SelectHero(this));
        }

        private void UpdateStarImages(Rarity rarity)
        {
            foreach (var star in _starImg)
            {
                star.gameObject.SetActive(false);
            }
            int starCount = 0;

            switch (rarity)
            {
                case Rarity.SS:
                    starCount = _starImg.Length;
                    break;
                case Rarity.S:
                    starCount = _starImg.Length - 1;
                    break;
                case Rarity.A:
                    starCount = _starImg.Length - 2;
                    break;
                case Rarity.B:
                    starCount = _starImg.Length - 3;
                    break;
                case Rarity.C:
                    starCount = 1;
                    break;
                default:
                    Debug.LogWarning($"Unknown rarity: {rarity}");
                    break;
            }
            for (int i = 0; i < starCount; i++)
            {
                _starImg[i].gameObject.SetActive(true);
            }
        }

        public void SetSelected(bool isSelected)
        {
            _selectedImg.gameObject.SetActive(isSelected);
        }
    }
}
