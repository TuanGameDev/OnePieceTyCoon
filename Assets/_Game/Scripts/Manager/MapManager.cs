using _Game.Scripts.Enums;
using _Game.Scripts.Helper;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Manager
{
    public class MapManager : Singleton<MapManager>
    {
        public MapDictionary MapDictionary;

        [SerializeField]
        private Transform _spawnPoint;

        private GameObject _currentSpawnedMap;

        public int CurrentMapIndex { get; private set; } = -1;

        private void Start()
        {
            SpawnMap(0);
        }

        public void SpawnMap(int mapIndex)
        {
            if (_currentSpawnedMap != null)
            {
                Destroy(_currentSpawnedMap);
                _currentSpawnedMap = null;
            }

            if (MapDictionary.TryGetValue(mapIndex, out var selectedMap))
            {
                if (selectedMap.MapObj != null)
                {
                    _currentSpawnedMap = Instantiate(
                        selectedMap.MapObj,
                        _spawnPoint.position,
                        Quaternion.identity,
                        _spawnPoint
                    );

                    CurrentMapIndex = mapIndex;
                }
            }
        }
    }
}

[System.Serializable]
public class MapDictionary : UnitySerializedDictionary<int, Map>
{
}

[System.Serializable]
public class Map
{
    public Sprite IconMap;
    public int Level;
    public GameObject MapObj;
    public Buy Buy;
}

[System.Serializable]
public class Buy
{
    public BuyState BuyState;
    public int Amount;
    public bool Purchased;
}
