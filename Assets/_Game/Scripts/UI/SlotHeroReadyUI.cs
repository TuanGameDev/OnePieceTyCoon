using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace _Game.Scripts.UI
{
    public class SlotHeroReadyUI : MonoBehaviour
    {
        [SerializeField]
        private Image _avatarImage;

        public void SetHeroUI(Sprite avatar)
        {
            _avatarImage.sprite = avatar;
        }
    }
}
