using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Game.Scripts.Manager;
namespace _Game.Scripts.UI
{
    public class LevelManagerUI : MonoBehaviour
    {
        [SerializeField]
        private Transform _container;

        [SerializeField]
        private SlotLevelUI _slotLevelUIPrefab;

        [SerializeField]
        private Button[] _villageButtons;

        void Start()
        {
            for (int i = 0; i < _villageButtons.Length; i++)
            {
                int index = i;
                _villageButtons[i].onClick.AddListener(() => LoadVillage(index));
            }
            LevelManager.Instance.LoadPlayerProgress();
            UpdateVillageButtons();
            DisplayLevelSlots();
        }

        private void LoadVillage(int villageIndex)
        {
            LevelManager.Instance.LoadVillage(villageIndex);

            UpdateVillageButtons();
            DisplayLevelSlots();
        }


        public void DisplayLevelSlots()
        {
            int currentLevelInVillage = LevelManager.Instance.CurrentLevelInVillage;
            string[] villageLevels = LevelManager.Instance.Villages[LevelManager.Instance.CurrentVillageIndex].Levels;

            foreach (Transform child in _container)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < villageLevels.Length; i++)
            {
                SlotLevelUI newSlot = Instantiate(_slotLevelUIPrefab, _container);

                if (i + 1 <= currentLevelInVillage)
                {
                    newSlot.SetLockState(false);
                    newSlot.GetComponent<Button>().onClick.AddListener(() => LevelManager.Instance.StartGame(i));
                }
                else
                {
                    newSlot.SetLockState(true);
                }

                newSlot.SetLevelName(villageLevels[i]);
            }
        }

        private void UpdateVillageButtons()
        {
            if (_villageButtons.Length == 0) return;

            _villageButtons[0].interactable = LevelManager.Instance.Villages[0].LockState;

            for (int i = 1; i < _villageButtons.Length; i++)
            {
                if (LevelManager.Instance.Villages[i].LockState)
                {
                    _villageButtons[i - 1].interactable = false;

                    _villageButtons[i].interactable = true;
                }
                else
                {
                    _villageButtons[i].interactable = false;
                }
            }
        }

    }
}
