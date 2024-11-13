using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace _Game.Scripts.UI
{
    public class TabHeroCtrlUI : MonoBehaviour
    {
        [SerializeField]
        private Button _showTabUIBtn;

        [SerializeField]
        private Animator _animator;

        private bool _isTabOpen = true;

        private void Start()
        {
            _showTabUIBtn.onClick.AddListener(ToggleTab);
        }

        private void ToggleTab()
        {
            if (_isTabOpen)
            {
                _animator.Play("TabOn");
            }
            else
            {
                _animator.Play("TabOff");
            }
            _isTabOpen = !_isTabOpen;
        }
    }
}
