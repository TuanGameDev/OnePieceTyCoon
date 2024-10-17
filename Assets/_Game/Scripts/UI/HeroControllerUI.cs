using _Game.Scripts.Character.Hero;
using _Game.Scripts.Manager;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.UI
{
    public class HeroControllerUI : MonoBehaviour
    {
        [SerializeField]
        private SlotHeroCtrlUI _slotHeroCtrUI;

        [SerializeField]
        private Transform _container;

        [SerializeField]
        private TurnBasedManager _turnBasedManager;

        void Start()
        {
            DisplayHeroImages();
        }

        void DisplayHeroImages()
        {
            foreach (Transform child in _container)
            {
                Destroy(child.gameObject);
            }
            for (int i = 0; i < _turnBasedManager.HeroControllers.Count; i++)
            {
                HeroController hero = _turnBasedManager.HeroControllers[i];
                if (hero != null)
                {
                    SlotHeroCtrlUI newSlot = Instantiate(_slotHeroCtrUI, _container);

                    newSlot.SetHeroCtrlUI(hero.HeroDataSO.HeroAvatar);
                }
            }
        }
    }
}
