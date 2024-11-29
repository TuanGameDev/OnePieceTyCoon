using _Game.Scripts.Character.Hero;
using _Game.Scripts.Manager;
using _Game.Scripts.Scriptable_Object;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using _Game.Scripts.Character;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

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

        [Space(20)]
        [SerializeField]
        private TextMeshProUGUI _nameBossTxt;

        [SerializeField]
        private TextMeshProUGUI _hpBossTxt;

        [SerializeField]
        private TextMeshProUGUI _rarityBossTxt;

        [SerializeField]
        private TextMeshProUGUI _timeBossTxt;

        [SerializeField]
        private TextMeshProUGUI _damageScoreTxt;

        [SerializeField]
        private TextMeshProUGUI _currentFightTxt;

        [Space(20)]
        [SerializeField]
        private Button _fightBtn;  
       
        [Space(20)]
        [SerializeField]
        private GameObject _panelBossPopup;

        [SerializeField]
        private GameObject _awardBossPopup;

        [Space(20)]
        [SerializeField]
        private Camera _cameraShake;

        [SerializeField]
        private float _timeBoss;

        [SerializeField]
        private int _currentFightAmount;
        [SerializeField] private int _maxFightAmount = 1;
        [SerializeField] private float _fightRecoveryTime = 24 * 60 * 60;
        [SerializeField] private List<double> _lastFightTimes = new List<double>();

        private void Awake()
        {
            _lastFightTimes.Clear();
            for (int i = 0; i < _maxFightAmount; i++)
            {
                string savedTime = PlayerPrefs.GetString($"LastFightTime_{i}", "0");
                _lastFightTimes.Add(double.Parse(savedTime));
            }
            UpdateFightAmount();
        }

        private void Start()
        {
            _fightBtn.onClick.AddListener(() => FightBoss().Forget());
            UpdateCountdownTimer().Forget();
        }

        private void Update()
        {
            _damageScoreTxt.text = "Damage Score: " + BossManager.Instance.PreviousScore.ToString("N0");
        }
        private async UniTaskVoid FightBoss()
        {
            if (BossManager.Instance == null || _currentFightAmount <= 0)
            {
                Debug.LogWarning("No fight attempts left!");
                return;
            }

            int index = _lastFightTimes.FindIndex(time => time == 0);
            if (index >= 0)
            {
                _lastFightTimes[index] = GetUnixTimestamp();
            }

            UpdateFightAmount();

            await ShowBossPopup();
        }


        private async UniTask ShowBossPopup()
        {
            if (_panelBossPopup == null || _timeBossTxt == null || _cameraShake == null || BossManager.Instance == null)
            {
                Debug.LogError("Missing required references or BossManager is null!");
                return;
            }

            _panelBossPopup.SetActive(true);
            _timeBossTxt.gameObject.SetActive(true);
            _timeBossTxt.text = "Boss is about to appear!";
            await ShakeCamera(1f, 0.2f);
            await UniTask.Delay(1500);

            BossManager.Instance.SpawnBoss(0);
            AudioManager.Instance.PlaySFX(1);
            AudioManager.Instance.StopSFX(0);
            var bossCtrl = BossManager.Instance.CurrentBossCtrl;
            if (bossCtrl != null && bossCtrl.Animator.HasState(0, Animator.StringToHash("IntroKaido")))
            {
                bossCtrl.Animator.Play("IntroKaido");
            }

            bossCtrl?.SetState(new WaitingState());
            UpdateUI();

            _panelBossPopup.SetActive(false);
            _cameraShake.transform.localPosition = new Vector3(0, 0, -10);

            float remainingTime = _timeBoss;
            while (remainingTime > 0)
            {
                _timeBossTxt.text = $"World boss appears: {remainingTime:N0}s";
                await UniTask.Delay(1000);
                remainingTime -= 1f;
            }

            _timeBossTxt.gameObject.SetActive(false);
            _awardBossPopup.gameObject.SetActive(true);
            AudioManager.Instance.PlaySFX(0);
            AudioManager.Instance.StopSFX(1);
            if (BossManager.Instance.CurrentBossCtrl != null)
            {
                Destroy(BossManager.Instance.CurrentBossCtrl.gameObject);
                BossManager.Instance.CurrentBossCtrl = null;
                _fightBtn.interactable = true;
                RankingBossUI.Instance.AddScoreBoss(BossManager.Instance.PreviousScore);
            }
        }

        private async UniTask ShakeCamera(float duration, float magnitude)
        {
            if (_cameraShake == null)
            {
                Debug.LogWarning("CameraShake is not assigned. Skipping shake effect.");
                return;
            }

            Vector3 originalPosition = _cameraShake.transform.localPosition;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                float offsetX = Random.Range(-1f, 1f) * magnitude;
                float offsetY = Random.Range(-1f, 1f) * magnitude;

                _cameraShake.transform.localPosition = new Vector3(
                    originalPosition.x + offsetX,
                    originalPosition.y + offsetY,
                    originalPosition.z
                );

                elapsedTime += Time.deltaTime;
                await UniTask.Yield();
            }

            _cameraShake.transform.localPosition = originalPosition;
        }

        public void UpdateUI()
        {
            if (BossManager.Instance == null || BossManager.Instance.CurrentBossCtrl == null)
            {
                Debug.LogWarning("BossManager or CurrentBossCtrl is null. Skipping UI update.");
                return;
            }

            var bossCtrl = BossManager.Instance.CurrentBossCtrl;
            var currentBossData = bossCtrl.HeroDataSO;

            if (currentBossData == null)
            {
                return;
            }

            _avatarBoss.gameObject.SetActive(true);
            _nameBossTxt.gameObject.SetActive(true);
            _rarityBossTxt.gameObject.SetActive(true);

            _avatarBoss.sprite = currentBossData.HeroAvatar;
            _nameBossTxt.text = BossManager.Instance.BossInfos.Find(info => info.HeroData == currentBossData)?.NameBoss ?? "Unknown Boss";
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
        }

        private void UpdateHealthBarSmooth()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            SmoothHealthBarChange().Forget();
        }

        private async UniTask SmoothHealthBarChange()
        {
            if (_hpBarImg == null || BossManager.Instance.CurrentBossCtrl == null) return;

            var bossCtrl = BossManager.Instance.CurrentBossCtrl;
            float maxHp = bossCtrl.HeroDataSO.CharacterStat.Hp;

            if (maxHp <= 0f) return;

            float targetFillAmount = (float)bossCtrl.CurrentHP / maxHp;
            float currentFillAmount = _hpBarImg.fillAmount;

            float duration = 0.1f;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                _hpBarImg.fillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, elapsedTime / duration);
                await UniTask.Yield();
            }

            _hpBarImg.fillAmount = targetFillAmount;
        }

        private async UniTaskVoid UpdateCountdownTimer()
        {
            while (true)
            {
                UpdateFightAmount();
                await UniTask.Delay(1000);
            }
        }

        private void UpdateFightAmount()
        {
            double currentTime = GetUnixTimestamp();
            double shortestTimeLeft = _fightRecoveryTime;

            for (int i = 0; i < _lastFightTimes.Count; i++)
            {
                if (_lastFightTimes[i] > 0)
                {
                    double timeLeft = _fightRecoveryTime - (currentTime - _lastFightTimes[i]);
                    if (timeLeft <= 0)
                    {
                        _lastFightTimes[i] = 0;
                    }
                    else
                    {
                        shortestTimeLeft = Mathf.Min((float)shortestTimeLeft, (float)timeLeft);
                    }
                }
            }

            _currentFightAmount = _maxFightAmount - _lastFightTimes.Count(time => time > 0);
            _currentFightTxt.text = $"{_currentFightAmount}/{_maxFightAmount}";

            if (_currentFightAmount < _maxFightAmount)
            {
                _currentFightTxt.text = FormatTime(shortestTimeLeft);
            }
            else
            {
                _currentFightTxt.text = $"{_currentFightAmount}/{_maxFightAmount}";
            }

            _fightBtn.interactable = _currentFightAmount > 0;
        }

        private string FormatTime(double timeInSeconds)
        {
            int hours = (int)(timeInSeconds / 3600);
            int minutes = (int)((timeInSeconds % 3600) / 60);
            int seconds = (int)(timeInSeconds % 60);
            return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }


        private double GetUnixTimestamp()
        {
            return (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1)).TotalSeconds;
        }

        private void OnApplicationQuit()
        {
            for (int i = 0; i < _lastFightTimes.Count; i++)
            {
                PlayerPrefs.SetString($"LastFightTime_{i}", _lastFightTimes[i].ToString());
            }
            PlayerPrefs.Save();
        }

        private void OnDestroy()
        {
            if (BossManager.Instance != null && BossManager.Instance.CurrentBossCtrl != null)
            {
                BossManager.Instance.CurrentBossCtrl.OnHealthChanged -= UpdateHealthBarSmooth;
                BossManager.Instance.CurrentBossCtrl.OnHealthChanged -= UpdateUI;
            }
        }
    }
}
