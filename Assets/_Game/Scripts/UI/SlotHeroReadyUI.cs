using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace _Game.Scripts.UI
{
    public class SlotHeroReadyUI : MonoBehaviour
    {
        [SerializeField]
        private Image _avatarImage;

        public void SetHeroUI(string avatarPath)
        {
            if (!string.IsNullOrEmpty(avatarPath))
            {
                Sprite avatarSprite = Resources.Load<Sprite>(avatarPath);
                if (avatarSprite != null)
                {
                    _avatarImage.sprite = avatarSprite;
                }
            }
        }
    }
}
