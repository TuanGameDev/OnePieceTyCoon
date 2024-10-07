using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Game.Scripts.Character.Hero;
using _Game.Scripts.Manager;

namespace _Game.Scripts.Manager
{
    public class SpawnHeroManager : MonoBehaviour
    {
        [SerializeField]
        private HeroController _heroCrl;  // Prefab của hero

        [SerializeField]
        public Transform[] SpawnPoints;  // Các điểm spawn hero

        [SerializeField]
        private TurnBasedManager turnBasedManager;  // Tham chiếu đến TurnBasedManager

        private void Start()
        {
            SpawnHeroes();
        }

        private void SpawnHeroes()
        {
            if (_heroCrl == null || turnBasedManager == null)
            {
                Debug.LogWarning("Hero prefab or TurnBasedManager reference is missing.");
                return;
            }

            for (int i = 0; i < SpawnPoints.Length; i++)
            {
                if (SpawnPoints[i] == null)
                {
                    Debug.LogWarning("Spawn point is null at index " + i);
                    continue;
                }

                HeroController heroInstance = Instantiate(_heroCrl, SpawnPoints[i].position, Quaternion.identity);

                // Thêm hero vào danh sách HeroTransforms trong TurnBasedManager
                turnBasedManager.HeroControllers.Add(heroInstance);
            }
        }
    }
}
