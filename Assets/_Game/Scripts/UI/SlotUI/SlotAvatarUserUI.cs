using UnityEngine;
using UnityEngine.UI;
using System;

namespace _Game.Scripts.UI
{
    public class SlotAvatarUserUI : MonoBehaviour
    {
        [SerializeField]
        private Image _avatarUser;

        [SerializeField]
        private Button _selectedBtn;

        public Action<Sprite> OnAvatarSelected;

        private void Start()
        {
            _selectedBtn.onClick.AddListener(OnSelectedButtonClick);
        }

        public void SetHeroUI(string avatarPath)
        {
            if (!string.IsNullOrEmpty(avatarPath))
            {
                Sprite avatarSprite = Resources.Load<Sprite>(avatarPath);
                if (avatarSprite != null)
                {
                    _avatarUser.sprite = avatarSprite;
                }
            }
        }

        private void OnSelectedButtonClick()
        {
            OnAvatarSelected?.Invoke(_avatarUser.sprite);
        }
    }
}
