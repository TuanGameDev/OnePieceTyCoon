using _Game.Scripts.Character.Hero;
using _Game.Scripts.Helper;
using _Game.Scripts.Manager;
using _Game.Scripts.Scriptable_Object;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.UI
{
    public class HeroControllerUI : Singleton<HeroControllerUI>
    {
        [SerializeField]
        private SlotHeroCtrlUI _slotHeroCtrlPrefab;

        [SerializeField]
        private Transform _container;

        public HeroConCrlUISlot HeroCtrlUISlot;

        private List<SlotHeroCtrlUI> _heroSlots = new List<SlotHeroCtrlUI>();
        public void UpdateDisplayHeroes()
        {
            foreach (Transform child in _container)
            {
                Destroy(child.gameObject);
            }
            _heroSlots.Clear();

            foreach (KeyValuePair<int, HeroController> heroPair in HeroCtrlUISlot)
            {
                var heroController = heroPair.Value;
                var heroDataSO = heroController.HeroDataSO;

                if (heroDataSO != null)
                {
                    var slotHero = Instantiate(_slotHeroCtrlPrefab, _container);
                    slotHero.SetHeroUI(heroDataSO.IconAvatarPath, heroController);
                    _heroSlots.Add(slotHero);
                }
            }
        }
    }

    [System.Serializable]
    public class HeroConCrlUISlot : UnitySerializedDictionary<int,HeroController>
    {

    }
}
