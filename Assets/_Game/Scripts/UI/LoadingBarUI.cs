using _Game.Scripts.Manager;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace _Game.Scripts.UI
{
    public class LoadingBarUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _loadingTxt;

        [SerializeField]
        private Image _barImg;

        [SerializeField]
        private float _speedLoading = 20f;

        private float _currentProgress = 0f;

        private void Start()
        {
            StartCoroutine(LoadProgress());
        }

        private IEnumerator LoadProgress()
        {
            while (_currentProgress < 100f)
            {
                _currentProgress += _speedLoading * Time.deltaTime;

                _currentProgress = Mathf.Clamp(_currentProgress, 0f, 100f);

                UpdateUI(_currentProgress);

                yield return null;
            }
            _currentProgress = 100f;
            UpdateUI(_currentProgress);
            LevelManager.Instance.StartGame(LevelManager.Instance.CurrentLevelInVillage);
            AudioManager.Instance.PlaySFX(1);
            AudioManager.Instance.StopSFX(0);
        }

        private void UpdateUI(float progress)
        {
            _loadingTxt.text = "Tham Chiến..." + Mathf.FloorToInt(progress).ToString() + "%";

            _barImg.fillAmount = progress / 100f;
        }
    }
}

