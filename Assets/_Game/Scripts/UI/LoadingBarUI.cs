using _Game.Scripts.Manager;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        public bool LoginBool;

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
            StartCoroutine(LoadScene(1f));
        }
        private void UpdateUI(float progress)
        {
            _loadingTxt.text = "Loading..." + Mathf.FloorToInt(progress).ToString() + "%";

            _barImg.fillAmount = progress / 100f;
        }

        private IEnumerator LoadScene(float delay)
        {
            yield return new WaitForSeconds(delay);
            SceneManager.LoadScene(1);
        }
    }
}
