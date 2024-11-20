using _Game.Scripts.Character;
using _Game.Scripts.Character.Hero;
using _Game.Scripts.Helper;
using _Game.Scripts.StatePatern;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.Buildings
{
    public class HouseRevive : Singleton<HouseRevive>
    {
        [SerializeField] private float _healthRegainDuration = 5f;
        [SerializeField] private Image Bar;
        public Transform ReviveHeroPoint;
        [SerializeField] private TextMeshProUGUI _timeTxt;
        [SerializeField] private GameObject _container;

        public void ReviveHero(HeroController hero)
        {
            if (!hero.IsReviving)
            {
                hero.IsReviving = true;
                StartCoroutine(ReviveCoroutine(hero));
            }
        }


        private IEnumerator ReviveCoroutine(HeroController hero)
        {
            //_container.SetActive(true);
            float elapsedTime = 0f;

            if (Bar != null)
            {
                Bar.fillAmount = 0f;
            }

            if (_timeTxt != null)
            {
                _timeTxt.text = _healthRegainDuration.ToString("F1") + "s";
            }

            while (elapsedTime < _healthRegainDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / _healthRegainDuration);

                if (Bar != null)
                {
                    Bar.fillAmount = progress;
                }

                if (_timeTxt != null)
                {
                    _timeTxt.text = (_healthRegainDuration - elapsedTime).ToString("F1") + "s";
                }

                yield return null;
            }

           // _container.SetActive(false);
            hero.gameObject.SetActive(true);
            hero.BaseRoot.SetActive(true);
            hero.CurrentHP = hero.CurrentStat.Hp;
            hero.GetComponent<BoxCollider2D>().enabled = true;
            hero.IsDead = false;

            hero.Animator.enabled = true;
            hero.transform.position = ReviveHeroPoint.position;

            if (Bar != null)
            {
                Bar.fillAmount = 1f;
            }

            if (_timeTxt != null)
            {
                _timeTxt.text = "0.0s";
            }

            hero.IsReviving = false;
            hero.IsNeedHeal = false;
            hero.SetState(new IdleState());
            hero.OnHealthChanged?.Invoke();
        }
    }
}
