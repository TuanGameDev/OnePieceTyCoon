using _Game.Scripts.Character.Hero;
using _Game.Scripts.Scriptable_Object;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
        private Coroutine _healthBarCoroutine;

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

            if (_heroCtrl != null)
            {
                _heroCtrl.OnHealthChanged.AddListener(UpdateHealthBarSmooth);
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            if (_heroCtrl == null) return;

            _levelHeroTxt.text = $"Lv. " + _heroCtrl.HeroDataSO.CharacterStat.HeroLevel;
            UpdateHealthBarSmooth();
            UpdateExpBar();
        }

        private void UpdateHealthBarSmooth()
        {
            if (_healthBarCoroutine != null)
            {
                StopCoroutine(_healthBarCoroutine);
            }

            _healthBarCoroutine = StartCoroutine(SmoothHealthBarChange());
        }

        private IEnumerator SmoothHealthBarChange()
        {
            if (_healthBar == null || _heroCtrl == null) yield break;

            float targetFillAmount = (float)_heroCtrl.CurrentHP / _heroCtrl.HeroDataSO.CharacterStat.Hp;
            float currentFillAmount = _healthBar.fillAmount;

            float duration = 0.1f;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                _healthBar.fillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, elapsedTime / duration);
                yield return null;
            }

            _healthBar.fillAmount = targetFillAmount;
        }

        private void UpdateExpBar()
        {
            if (_expBar != null && _heroCtrl != null)
            {
                _expBar.fillAmount = (float)_heroCtrl.HeroDataSO.CharacterStat.CurrentExp / _heroCtrl.HeroDataSO.CharacterStat.MaxExp;
            }
        }

        private void OnDestroy()
        {
            if (_heroCtrl != null)
            {
                _heroCtrl.OnHealthChanged.RemoveListener(UpdateHealthBarSmooth);
            }
        }
    }
}
