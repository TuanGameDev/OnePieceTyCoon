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
        private Button _startGameBtn;

        [SerializeField]
        private Button[] _villageButtons;

        [SerializeField]
        private TextMeshProUGUI _villageNameText;

        void Start()
        {
            for (int i = 0; i < _villageButtons.Length; i++)
            {
                int index = i;
                _villageButtons[i].onClick.AddListener(() => LoadVillage(index));
            }
            _startGameBtn.onClick.AddListener(() => LevelManager.Instance.StartGame(LevelManager.Instance.CurrentLevelInVillage));

            LevelManager.Instance.LoadPlayerProgress();
            UpdateVillageButtons();
            DisplayLevelSlots();
        }

        private void LoadVillage(int villageIndex)
        {
            LevelManager.Instance.LoadVillage(villageIndex);
            _villageNameText.text = LevelManager.Instance.Villages[villageIndex].VillageName;

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
            // Nếu không có làng nào, thoát khỏi hàm
            if (_villageButtons.Length == 0) return;

            // Nút làng đầu tiên luôn có thể tương tác
            _villageButtons[0].interactable = LevelManager.Instance.Villages[0].LockState;

            // Kiểm tra và cập nhật trạng thái các nút làng
            for (int i = 1; i < _villageButtons.Length; i++)
            {
                if (LevelManager.Instance.Villages[i].LockState)
                {
                    // Nếu LockState của làng hiện tại là true
                    // Khóa nút của làng trước
                    _villageButtons[i - 1].interactable = false;

                    // Mở nút của làng hiện tại
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
