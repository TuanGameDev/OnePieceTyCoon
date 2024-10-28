using UnityEngine;
using UnityEngine.UI;
using _Game.Scripts.Scriptable_Object;
using _Game.Scripts.Manager;
using System.Collections.Generic;
using _Game.Scripts.Character.Hero;

namespace _Game.Scripts.UI
{
    public class SlotHeroUI : MonoBehaviour
    {
        [SerializeField]
        private Image _avatarImage;

        private HeroData _heroData;

        public void SetHeroUI(string avatarPath, HeroData heroData)
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
