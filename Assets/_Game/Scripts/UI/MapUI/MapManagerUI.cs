using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using _Game.Scripts.Manager;
using _Game.Scripts.Enums;
using PlayFab.ClientModels;
using PlayFab;
using TMPro;
using _Game.Scripts.Helper;

namespace _Game.Scripts.UI
{
    public class MapManagerUI : MonoBehaviour
    {
        [SerializeField]
        private StatusIcon _statesIcon;

        [SerializeField]
        private Image _mapBG;

        [SerializeField]
        private Image _iconLock;

        [SerializeField]
        private Image _iconStatus;

        [SerializeField]
        private TextMeshProUGUI _amountMapTxt;

        [SerializeField]
        private TextMeshProUGUI _statMapTxt;

        [SerializeField]
        private TextMeshProUGUI _messageTxt;

        [SerializeField]
        private Button _useBtn;

        [SerializeField]
        private Button _buyMapBtn;

        [SerializeField]
        private Button _changeNext;

        [SerializeField]
        private Button _changeBack;

        private int _currentMapIndex;

        private void Start()
        {
            _changeNext.onClick.AddListener(ChangeNext);
            _changeBack.onClick.AddListener(ChangeBack);
            _buyMapBtn.onClick.AddListener(SelectMap);
            _useBtn.onClick.AddListener(UseMap);

            _currentMapIndex = 0;
            UpdateUI();
            LoadDataMap();
        }

        private void OnEnable()
        {
            UpdateUI();
        }

        private void ChangeNext()
        {
            if (_currentMapIndex < MapManager.Instance.MapDictionary.Count - 1)
            {
                _currentMapIndex++;
                UpdateUI();
            }
        }

        private void ChangeBack()
        {
            if (_currentMapIndex > 0)
            {
                _currentMapIndex--;
                UpdateUI();
            }
        }
        private void UseMap()
        {
            if (MapManager.Instance.MapDictionary.TryGetValue(_currentMapIndex, out var map))
            {
                if (map.Buy.Purchased)
                {
                    MapManager.Instance.SpawnMap(_currentMapIndex);
                    UpdateUI();
                }
            }
        }

        private void SelectMap()
        {
            if (MapManager.Instance.MapDictionary.TryGetValue(_currentMapIndex, out var map))
            {
                var userInfo = RankingManager.Instance.UserInformation;

                bool canAfford = map.Buy.BuyState switch
                {
                    BuyState.Beli => userInfo.Beli >= map.Buy.Amount,
                    BuyState.Diamond => userInfo.Diamond >= map.Buy.Amount,
                    _ => false
                };

                if (!canAfford)
                {
                    _messageTxt.text = "You don't have enough currency to buy this map!";
                    _messageTxt.color = Color.red;
                    _messageTxt.gameObject.SetActive(true);
                    Invoke(nameof(HideMessage), 3f);
                    return;
                }

                if (canAfford && !map.Buy.Purchased)
                {
                    switch (map.Buy.BuyState)
                    {
                        case BuyState.Beli:
                            userInfo.Beli -= map.Buy.Amount;
                            break;
                        case BuyState.Diamond:
                            userInfo.Diamond -= map.Buy.Amount;
                            break;
                    }

                    map.Buy.Purchased = true;
                    SaveDataMap();
                    UpdateUI();
                }
            }
        }

        private void HideMessage()
        {
            _messageTxt.gameObject.SetActive(false);
        }

        private void LoadDataMap()
        {
            if (string.IsNullOrEmpty(RankingManager.Instance.UserInformation.MasterPlayerID))
            {
                Debug.LogError("PlayFab ID chưa được thiết lập.");
                return;
            }

            PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
                result =>
                {
                    if (result.Data != null && result.Data.ContainsKey("MapData"))
                    {
                        string mapDataJson = result.Data["MapData"].Value;

                        var wrapper = JsonUtility.FromJson<SerializationWrapper<int, bool>>(mapDataJson);
                        var mapData = wrapper.ToDictionary();

                        foreach (var entry in mapData)
                        {
                            if (MapManager.Instance.MapDictionary.TryGetValue(entry.Key, out var map))
                            {
                                map.Buy.Purchased = entry.Value;
                            }
                        }
                        UpdateUI();
                    }
                },
                error => Debug.LogError($"Failed to load map data: {error.ErrorMessage}"));
        }

        private void SaveDataMap()
        {
            if (string.IsNullOrEmpty(RankingManager.Instance.UserInformation.MasterPlayerID))
            {
                Debug.LogError("PlayFab ID chưa được thiết lập.");
                return;
            }
            var mapData = new Dictionary<int, bool>();
            foreach (var entry in MapManager.Instance.MapDictionary)
            {
                mapData[entry.Key] = entry.Value.Buy.Purchased;
            }

            string mapDataJson = JsonUtility.ToJson(new SerializationWrapper<int, bool>(mapData));

            var request = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
        {
            { "MapData", mapDataJson }
        }
            };

            PlayFabClientAPI.UpdateUserData(request,
                result => Debug.Log("Map data saved successfully!"),
                error => Debug.LogError($"Failed to save map data: {error.ErrorMessage}"));
        }


        private void UpdateUI()
        {
            if (MapManager.Instance.MapDictionary.TryGetValue(_currentMapIndex, out var map))
            {
                _mapBG.sprite = map.IconMap;
                _mapBG.enabled = map.IconMap != null;
                _amountMapTxt.text = map.Buy.Amount.ToString("N0");

                bool isPurchased = map.Buy.Purchased;
                _iconLock.gameObject.SetActive(!isPurchased);

                if (!isPurchased)
                {
                    if (ColorUtility.TryParseHtmlString("#AEAEAE", out var lockedColor))
                    {
                        _mapBG.color = lockedColor;
                    }
                }
                else
                {
                    _mapBG.color = Color.white;
                }

                _amountMapTxt.gameObject.SetActive(!isPurchased);
                _useBtn.gameObject.SetActive(isPurchased);
                _buyMapBtn.gameObject.SetActive(!isPurchased);

                if (MapManager.Instance.CurrentMapIndex == _currentMapIndex)
                {
                    _useBtn.interactable = false;
                    _statMapTxt.gameObject.SetActive(true);
                    _statMapTxt.text = "In Use";

                    if (ColorUtility.TryParseHtmlString("#AEAEAE", out var inUseColor))
                    {
                        _mapBG.color = inUseColor;
                    }
                }
                else
                {
                    _useBtn.interactable = true;
                    _statMapTxt.gameObject.SetActive(false);
                }

                if (_statesIcon.TryGetValue(map.Buy.BuyState, out var stateSprite))
                {
                    _iconStatus.sprite = stateSprite;
                    _iconStatus.enabled = true;
                }
                else
                {
                    _iconStatus.enabled = false;
                }

                var userInfo = RankingManager.Instance.UserInformation;
                bool canAfford = map.Buy.BuyState switch
                {
                    BuyState.Beli => userInfo.Beli >= map.Buy.Amount,
                    BuyState.Diamond => userInfo.Diamond >= map.Buy.Amount,
                    _ => false
                };

                //_buyMapBtn.interactable = !isPurchased && canAfford;
            }
        }


        [System.Serializable]
        public class StatusIcon: UnitySerializedDictionary<BuyState,Sprite>
        {

        }


        [System.Serializable]
        public class SerializationWrapper<TKey, TValue>
        {
            public List<TKey> keys = new List<TKey>();
            public List<TValue> values = new List<TValue>();

            public SerializationWrapper(Dictionary<TKey, TValue> dictionary)
            {
                foreach (var kvp in dictionary)
                {
                    keys.Add(kvp.Key);
                    values.Add(kvp.Value);
                }
            }

            public Dictionary<TKey, TValue> ToDictionary()
            {
                var dictionary = new Dictionary<TKey, TValue>();
                for (int i = 0; i < keys.Count; i++)
                {
                    dictionary[keys[i]] = values[i];
                }
                return dictionary;
            }
        }
    }
}
