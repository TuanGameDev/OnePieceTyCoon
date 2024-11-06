using _Game.Scripts.Enums;
using _Game.Scripts.Helper;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using _Game.Scripts.Manager;

namespace _Game.Scripts.UI.Hero
{
    public class ArrangeHeroesControllerUI : MonoBehaviour
    {
        [SerializeField]
        private TabButtonArrangeHeroes _tabButtonArrangeHeroes;

        [SerializeField]
        private Button _currentSelectedButton;

        [SerializeField]
        private Vector3 _normalButtonScale = new Vector3(0.8f, 0.8f, 0.8f);

        [SerializeField]
        private Vector3 _enlargedButtonScale = new Vector3(1f, 1f, 1f);

        private void Start()
        {
            foreach (var button in _tabButtonArrangeHeroes.Keys)
            {
                var elemental = _tabButtonArrangeHeroes[button];
                button.onClick.AddListener(() =>
                {
                    HandleButtonClick(button, elemental);
                });
            }
        }

        private void HandleButtonClick(Button button, Elemental elemental)
        {
            if (_currentSelectedButton == button)
            {
                button.transform.localScale = _normalButtonScale;
                ShowAllHeroes();

                _currentSelectedButton = null;
            }
            else
            {
                ShowHeroesByElemental(elemental);

                if (_currentSelectedButton != null)
                {
                    _currentSelectedButton.transform.localScale = _normalButtonScale;
                }
                _currentSelectedButton = button;
                button.transform.localScale = _enlargedButtonScale;
            }
        }


        private void ShowAllHeroes()
        {
            HeroesUI.Instance.LoadAndDisplayHeroes();
        }

        private void ShowHeroesByElemental(Elemental elemental)
        {
            var filteredHeroes = HeroManager.Instance.HeroesAvailable
                .SelectMany(heroList => heroList.heroes)
                .Where(hero => hero.Elemental == elemental)
                .OrderBy(hero => hero.Rarity)
                .ToList();

            HeroesUI.Instance.DisplayHeroes(filteredHeroes);
        }
    }

    [System.Serializable]
    public class TabButtonArrangeHeroes : UnitySerializedDictionary<Button, Elemental>
    {
    }
}
