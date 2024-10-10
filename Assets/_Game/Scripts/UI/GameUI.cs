using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace _Game.Scripts.UI
{
    public class GameUI : MonoBehaviour
    {

        public Button StartGameBtn;

        private void Start()
        {
            StartGameBtn.onClick.AddListener(StartGame);
        }
        public void StartGame()
        {
            SceneManager.LoadScene(1);
        }
    }
}
