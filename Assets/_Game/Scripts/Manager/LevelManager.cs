using _Game.Scripts.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class VillageLevels
{
    public string VillageName;
    public string[] Levels;
    public bool[] LevelCompleted;
    public bool LockState;
}

[System.Serializable]
public class SceneMap
{
    public int VillageIndex;
    public int LevelIndex;
    public GameObject Map; 
}

namespace _Game.Scripts.Manager
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;

        public List<VillageLevels> Villages;
        public List<SceneMap> SceneMaps;

        public int CurrentVillageIndex = 0;
        public int CurrentLevelInVillage = 0;

        private string _progressKey = "PlayerProgress";

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            LoadPlayerProgress();
            CheckVillageUnlock();
        }
        public void LoadVillage(int villageIndex)
        {
            if (villageIndex < Villages.Count)
            {
                CurrentVillageIndex = villageIndex;

                if (CurrentLevelInVillage >= Villages[CurrentVillageIndex].Levels.Length)
                {
                    CurrentLevelInVillage = Villages[CurrentVillageIndex].Levels.Length - 1;
                }
            }
        }


        public void StartGame(int levelIndex)
        {
            if (levelIndex < Villages[CurrentVillageIndex].Levels.Length)
            {
                CurrentLevelInVillage = levelIndex;
                SceneManager.LoadScene("Game");
                StartCoroutine(LoadMapWithDelay(CurrentVillageIndex, CurrentLevelInVillage));
            }
        }


        private IEnumerator LoadMapWithDelay(int villageIndex, int levelIndex)
        {
            yield return new WaitForSeconds(1f);

            LoadMap(villageIndex, levelIndex);
        }

        private void LoadMap(int villageIndex, int levelIndex)
        {
            foreach (SceneMap sceneMap in SceneMaps)
            {
                if (sceneMap.VillageIndex == villageIndex && sceneMap.LevelIndex == levelIndex)
                {
                    Instantiate(sceneMap.Map, Vector3.zero, Quaternion.identity);
                    Debug.Log($"Instantiated map for Village {villageIndex}, Level {levelIndex}");
                    break;
                }
            }
        }


        public void CompleteLevel()
        {
            Villages[CurrentVillageIndex].LevelCompleted[CurrentLevelInVillage] = true;
            CurrentLevelInVillage++;

            if (CurrentLevelInVillage >= Villages[CurrentVillageIndex].Levels.Length)
            {
                UnlockNextVillage();
            }
            else
            {
                SavePlayerProgress();
            }
        }

        public void UnlockNextVillage()
        {
            // Khóa làng hiện tại và tất cả các làng trước đó
            for (int i = 0; i <= CurrentVillageIndex; i++)
            {
                Villages[i].LockState = false; // Khóa tất cả các làng đã hoàn thành
            }

            // Kiểm tra xem có làng tiếp theo không
            if (CurrentVillageIndex + 1 < Villages.Count)
            {
                CurrentVillageIndex++; // Chuyển sang làng tiếp theo
                Villages[CurrentVillageIndex].LockState = true; // Mở khóa làng tiếp theo
                CurrentLevelInVillage = 0; // Reset cấp độ về 0 cho làng mới
                SavePlayerProgress();
            }
        }

        public void SavePlayerProgress()
        {
            PlayerPrefs.SetInt(_progressKey + "_Village", CurrentVillageIndex);
            PlayerPrefs.SetInt(_progressKey + "_Level", CurrentLevelInVillage);

            // Lưu trạng thái hoàn thành của các cấp độ
            for (int i = 0; i < Villages.Count; i++)
            {
                for (int j = 0; j < Villages[i].Levels.Length; j++)
                {
                    PlayerPrefs.SetInt($"{_progressKey}_LevelCompleted_{i}_{j}", Villages[i].LevelCompleted[j] ? 1 : 0);
                }
            }

            PlayerPrefs.Save();
        }

        public void LoadPlayerProgress()
        {
            if (PlayerPrefs.HasKey(_progressKey + "_Village") && PlayerPrefs.HasKey(_progressKey + "_Level"))
            {
                CurrentVillageIndex = PlayerPrefs.GetInt(_progressKey + "_Village");
                CurrentLevelInVillage = PlayerPrefs.GetInt(_progressKey + "_Level");
            }
            else
            {
                CurrentVillageIndex = 0;
                CurrentLevelInVillage = 0;
            }

            // Tải trạng thái hoàn thành cho từng cấp độ
            for (int i = 0; i < Villages.Count; i++)
            {
                Villages[i].LevelCompleted = new bool[Villages[i].Levels.Length];
                for (int j = 0; j < Villages[i].Levels.Length; j++)
                {
                    Villages[i].LevelCompleted[j] = PlayerPrefs.GetInt($"{_progressKey}_LevelCompleted_{i}_{j}", 0) == 1;
                }
            }
        }

        public void ResetProgress()
        {
            PlayerPrefs.DeleteKey(_progressKey + "_Village");
            PlayerPrefs.DeleteKey(_progressKey + "_Level");

            // Xóa trạng thái hoàn thành các cấp độ
            for (int i = 0; i < Villages.Count; i++)
            {
                for (int j = 0; j < Villages[i].Levels.Length; j++)
                {
                    PlayerPrefs.DeleteKey($"{_progressKey}_LevelCompleted_{i}_{j}");
                }
            }

            CurrentVillageIndex = 0;
            CurrentLevelInVillage = 0;
            Debug.Log("Progress reset!");
        }

        private void CheckVillageUnlock()
        {
            for (int i = 0; i < Villages.Count; i++)
            {
                if (i == 0)
                {
                    Villages[i].LockState = true; // Làng đầu tiên luôn mở
                }
                else
                {
                    // Kiểm tra xem tất cả cấp độ trong làng trước đó đã hoàn thành chưa
                    bool allLevelsCompleted = true;
                    for (int j = 0; j < Villages[i - 1].Levels.Length; j++)
                    {
                        if (!Villages[i - 1].LevelCompleted[j])
                        {
                            allLevelsCompleted = false;
                            break;
                        }
                    }
                    Villages[i].LockState = allLevelsCompleted; // Mở khóa nếu tất cả cấp độ đã hoàn thành
                }

                // Luôn luôn mở khóa cấp độ đầu tiên
                if (Villages[i].LevelCompleted.Length > 0) // Kiểm tra nếu có cấp độ nào
                {
                    Villages[i].LevelCompleted[0] = true; // Đánh dấu cấp độ đầu tiên là hoàn thành
                }
            }
        }
    }
}
