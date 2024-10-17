using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using _Game.Scripts.Manager;
namespace _Game.Scripts.UI
{
    public class GameManagerUI : MonoBehaviour
    {
        public TextMeshProUGUI GameStatTxt;

        public TextMeshProUGUI TimerTxt;

        public GameObject PanelPopup;

        [SerializeField]
        private Button _closeBtn;
        void Start()
        {
            _closeBtn.onClick.AddListener(CoutinueGame);
        }
        private void CoutinueGame()
        {
            SceneManager.LoadScene(1);
            Time.timeScale = 1;
            LevelManager.Instance.CompleteLevel();
            AudioManager.Instance.PlaySFX(0);
            AudioManager.Instance.StopSFX(1);
        }
    }
}
