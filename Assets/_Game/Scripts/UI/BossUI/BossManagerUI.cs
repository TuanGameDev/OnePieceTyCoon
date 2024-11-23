using _Game.Scripts.Character.Hero;
using _Game.Scripts.Manager;
using _Game.Scripts.Scriptable_Object;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI
{
    public class BossManagerUI : MonoBehaviour
    {
        [SerializeField]
        private RarityAndColorDictionary _rarityAndColorDictionary;

        [SerializeField]
        private Image _avatarBoss;

        [SerializeField]
        private Image _hpBarImg;

        [SerializeField]
        private TextMeshProUGUI _nameBossTxt;

        [SerializeField]
        private TextMeshProUGUI _hpBossTxt;

        [SerializeField]
        private TextMeshProUGUI _rarityBossTxt;

        private Coroutine _healthBarCoroutine;

        [SerializeField]
        private Button _fightBtn;

        [SerializeField]
        private float _timeBoss;

        private void Start()
        {
            _fightBtn.onClick.AddListener(FightBoss);
        }

        private void OnEnable()
        {
            UpdateUI();
        }

        private void FightBoss()
        {
            BossManager.Instance.SpawnBoss(0);
            UpdateUI();
            if (BossManager.Instance.CurrentBossCtrl != null)
            {
                StartCoroutine(DestroyBossAfterTime(_timeBoss));
            }
        }

        private IEnumerator DestroyBossAfterTime(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (BossManager.Instance.CurrentBossCtrl != null)
            {
                Destroy(BossManager.Instance.CurrentBossCtrl.gameObject);
                BossManager.Instance.CurrentBossCtrl = null;
            }
        }

        private void UpdateUI()
        {
            if (BossManager.Instance != null && BossManager.Instance.CurrentBossCtrl != null)
            {
                var bossCtrl = BossManager.Instance.CurrentBossCtrl;
                var currentBossData = bossCtrl.HeroDataSO;

                foreach (var bossPair in BossManager.Instance.Boss)
                {
                    string bossKey = bossPair.Key;
                    HeroDataSO bossData = bossPair.Value;

                    if (bossData == currentBossData)
                    {
                        _avatarBoss.gameObject.SetActive(true);
                        _nameBossTxt.gameObject.SetActive(true);
                        _rarityBossTxt.gameObject.SetActive(true);

                        _avatarBoss.sprite = currentBossData.HeroAvatar;
                        _nameBossTxt.text = bossKey;
                        _rarityBossTxt.text = currentBossData.Rarity.ToString();
                        _rarityBossTxt.color = _rarityAndColorDictionary.TryGetValue(currentBossData.Rarity, out Color color) ? color : Color.white;
                        _hpBossTxt.text = $"{bossCtrl.CurrentHP:N0}";
                        _hpBarImg.fillAmount = (float)bossCtrl.CurrentHP / currentBossData.CharacterStat.Hp;

                        bossCtrl.OnHealthChanged -= UpdateHealthBarSmooth;
                        bossCtrl.OnHealthChanged -= UpdateUI;
                        bossCtrl.OnHealthChanged += UpdateHealthBarSmooth;
                        bossCtrl.OnHealthChanged += UpdateUI;

                        UpdateHealthBarSmooth();
                        _fightBtn.interactable = false;

                        return;
                    }
                }
            }
            else
            {
                _avatarBoss.gameObject.SetActive(false);
                _nameBossTxt.gameObject.SetActive(false);
                _rarityBossTxt.gameObject.SetActive(false);
                _rarityBossTxt.color = Color.white;
                _fightBtn.interactable = true;
            }
        }


        private void UpdateHealthBarSmooth()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            if (_healthBarCoroutine != null)
            {
                StopCoroutine(_healthBarCoroutine);
            }

            _healthBarCoroutine = StartCoroutine(SmoothHealthBarChange());
        }


        private IEnumerator SmoothHealthBarChange()
        {
            if (_hpBarImg == null || BossManager.Instance.CurrentBossCtrl == null) yield break;

            var bossCtrl = BossManager.Instance.CurrentBossCtrl;
            float maxHp = bossCtrl.HeroDataSO.CharacterStat.Hp;

            if (maxHp <= 0f) yield break;

            float targetFillAmount = (float)bossCtrl.CurrentHP / maxHp;
            float currentFillAmount = _hpBarImg.fillAmount;

            float duration = 0.1f;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                _hpBarImg.fillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, elapsedTime / duration);
                yield return null;
            }

            _hpBarImg.fillAmount = targetFillAmount;
        }


        private void OnDestroy()
        {
            if (BossManager.Instance.CurrentBossCtrl != null)
            {
                BossManager.Instance.CurrentBossCtrl.OnHealthChanged -= UpdateHealthBarSmooth;
                BossManager.Instance.CurrentBossCtrl.OnHealthChanged -= UpdateUI;
            }
        }
    }
}
