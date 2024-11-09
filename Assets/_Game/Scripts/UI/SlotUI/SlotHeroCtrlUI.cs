using _Game.Scripts.Character.Hero;
using _Game.Scripts.Scriptable_Object;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI
{
    public class SlotHeroCtrlUI : MonoBehaviour
    {
        [SerializeField]
        private Image _avatarHero;

        [SerializeField]
        private Image _healthBar;

        [SerializeField]
        private Image _expBar;

        [SerializeField]
        private TextMeshProUGUI _levelHeroTxt;

        private HeroController _heroCtrl;

        private void Start()
        {
            UpdateUI();
        }

        public void SetHeroUI(string avatarPath, HeroController heroDataSO)
        {
            if (!string.IsNullOrEmpty(avatarPath))
            {
                Sprite avatarSprite = Resources.Load<Sprite>(avatarPath);
                if (avatarSprite != null)
                {
                    _avatarHero.sprite = avatarSprite;
                }
            }

            _heroCtrl = heroDataSO;
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (_heroCtrl == null) return;

            _levelHeroTxt.text = $"Lv. " + _heroCtrl.HeroDataSO.CharacterStat.HeroLevel;
            if (_healthBar != null)
            {
                _healthBar.fillAmount = _heroCtrl.CurrentHP / _heroCtrl.HeroDataSO.CharacterStat.Hp;
            }
            if (_expBar != null)
            {
                _expBar.fillAmount = _heroCtrl.HeroDataSO.CharacterStat.CurrentExp / _heroCtrl.HeroDataSO.CharacterStat.MaxExp;
            }
        }
    }
}
