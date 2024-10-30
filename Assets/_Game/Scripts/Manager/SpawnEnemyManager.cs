using _Game.Scripts.Non_Mono;
using UnityEngine;
using _Game.Scripts.Scriptable_Object.Enemy;
using _Game.Scripts.Character.Eenmy;
using _Game.Scripts.Enums;

namespace _Game.Scripts.Manager
{
    public class SpawnEnemyManager : MonoBehaviour
    {
        [SerializeField]
        private EnemyController _enemyPrefab;

        [SerializeField]
        private Transform[] _spawnPoints;


        private void Start()
        {
            SpawnEnemies();
        }

        public void SpawnEnemies()
        {
           
        }
    }
}
