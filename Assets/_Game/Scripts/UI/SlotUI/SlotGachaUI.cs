using _Game.Scripts.Scriptable_Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace _Game.Scripts.UI
{
    public class SlotGachaUI : MonoBehaviour
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
