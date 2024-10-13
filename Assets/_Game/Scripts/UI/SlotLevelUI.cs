using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI
{
    public class SlotLevelUI : MonoBehaviour
    {
        public Image Unlock;
        public Image Lock;

        public TextMeshProUGUI NameLevelTxt;

        public bool LockState;

        public void UpdateUI()
        {
            if (LockState)
            {
                Lock.gameObject.SetActive(true);
                Unlock.gameObject.SetActive(false);
            }
            else
            {
                Lock.gameObject.SetActive(false);
                Unlock.gameObject.SetActive(true);
            }
        }

        public void SetLockState(bool isLocked)
        {
            LockState = isLocked;
            UpdateUI();
        }

        public void SetLevelName(string levelName)
        {
            NameLevelTxt.text = levelName;
        }
    }
}
