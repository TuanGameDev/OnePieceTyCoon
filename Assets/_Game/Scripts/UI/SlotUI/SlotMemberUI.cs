using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI
{
    public class SlotMemberUI : MonoBehaviour
    {
        [SerializeField]
        private Image _avatarMember;

        [SerializeField]
        private TextMeshProUGUI _positionMemberTxt;

        [SerializeField]
        private TextMeshProUGUI _nameMemberTxt;

        [SerializeField]
        private TextMeshProUGUI _powerMemberTxt;

        public Button OrdainBtn;

        public void SetData(string position, string name, Sprite avatarSprite)
        {
            _positionMemberTxt.text = $"Role: {position}";
            _nameMemberTxt.text = name;

            if (avatarSprite != null)
            {
                _avatarMember.sprite = avatarSprite;
            }
            else
            {
                _avatarMember.color = Color.gray;
            }
        }

        public void ShowOrdainButton(bool show)
        {
            OrdainBtn.gameObject.SetActive(show);
        }
    }
}
