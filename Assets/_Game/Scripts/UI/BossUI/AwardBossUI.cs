using _Game.Scripts.Manager;
using _Game.Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace _Game.Scripts.UI
{
    public class AwardBossUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _totalScoreTxt;

        [SerializeField]
        private TextMeshProUGUI _beliTxt;

        [SerializeField]
        private TextMeshProUGUI _diamondTxt;

        private void OnEnable()
        {
            UpdateAward();
        }

        private void UpdateAward()
        {
            RankingManager.Instance.UserInformation.Beli += 1000;
            RankingManager.Instance.UserInformation.Beli += 100;

            RankingManager.Instance.UpdateBeliAndDiamondText(_beliTxt, _diamondTxt);

            UserManagerUI.Instance.SaveUserInformation();
            _totalScoreTxt.text = "Total Score: " + BossManager.Instance.PreviousScore.ToString("N0");
        }
    }
}
