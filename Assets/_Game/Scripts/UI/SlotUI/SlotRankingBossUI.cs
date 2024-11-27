using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI
{
    public class SlotRankingBossUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _topUserTxt;

        [SerializeField]
        private Image _avatarUser;

        [SerializeField]
        private TextMeshProUGUI _nameUserTxt;

        [SerializeField]
        private TextMeshProUGUI _scoreUserTxt;

        public void SetData(int top, string name, int score, Sprite avatarSprite)
        {
            _topUserTxt.text = $"Top {top}";
            _nameUserTxt.text = name;
            _scoreUserTxt.text = $"Score: {score.ToString("N0")}";

            if (avatarSprite != null)
            {
                _avatarUser.sprite = avatarSprite;
            }
        }
    }
}
