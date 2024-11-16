using _Game.Scripts.Enums;
using _Game.Scripts.Non_Mono;
using _Game.Scripts.Scriptable_Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace _Game.Scripts.UI
{
    public class SlotGachaUI : MonoBehaviour
    {
        [SerializeField]
        private ElementalImgDictionary _elementalImgDictionary;

        [SerializeField]
        private Image[] _starImg;

        [SerializeField]
        private Image _avatarImage;

        [SerializeField]
        private Image _elementalHero;

        public HeroData HeroData;
        public void SetHeroUI(string avatarPath, HeroData heroData)
        {
            if (!string.IsNullOrEmpty(avatarPath))
            {
                Sprite avatarSprite = Resources.Load<Sprite>(avatarPath);
                if (avatarSprite != null)
                {
                    _avatarImage.sprite = avatarSprite;
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

                UpdateStarImages(heroData.Rarity);

            }
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
    }
}
