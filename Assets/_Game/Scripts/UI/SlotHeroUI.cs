using UnityEngine;
using UnityEngine.UI;
using _Game.Scripts.Scriptable_Object;

namespace _Game.Scripts.UI
{
    public class SlotHeroUI : MonoBehaviour
    {
        [SerializeField]  
        private Image _avatarImage;

        [SerializeField] 
        private Image _selectedImg;

        [SerializeField] 
        private Button _selectedBtn;

        [SerializeField] 
        private HeroData _heroData;

        public bool IsInCombat;

        public HeroData HeroData => _heroData;

        public void SetHeroUI(string avatarPath, HeroData heroData, HeroesUI heroesUI)
        {
            if (!string.IsNullOrEmpty(avatarPath))
            {
                Sprite avatarSprite = Resources.Load<Sprite>(avatarPath);
                if (avatarSprite != null)
                {
                    _avatarImage.sprite = avatarSprite;
                    _heroData = heroData;
                }
            }
            _selectedImg.gameObject.SetActive(false);
            _selectedBtn.onClick.AddListener(() => heroesUI.SelectHero(this));
        }

        public void SetSelected(bool isSelected)
        {
            _selectedImg.gameObject.SetActive(isSelected);
        }
    }
}
