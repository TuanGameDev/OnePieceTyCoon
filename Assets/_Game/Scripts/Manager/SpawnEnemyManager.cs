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
        private EnemyOutLook _enemyOutLook;

        [SerializeField]
        private Transform[] _spawnPoints;

        [SerializeField]
        private EnemyDictionary _enemyDictionary;

        [SerializeField]
        private TurnBasedManager _turnbasedManager;

        private void Start()
        {
            SpawnEnemies();
        }

        public void SpawnEnemies()
        {
            int spawnIndex = 0;

            foreach (var enemyData in _enemyDictionary)
            {
                EnemysName enemyName = enemyData.Key.EnemyName;
                Rank enemyRank = enemyData.Key.Rank;

                EnemyNameAndRank key = new EnemyNameAndRank(enemyName, enemyRank);

                if (_enemyOutLook.EnemyOut.TryGetValue(key, out OutLook outLook))
                {
                    if (outLook.Root != null)
                    {
                        if (spawnIndex < _spawnPoints.Length)
                        {
                            EnemyController enemyInstance = Instantiate(_enemyPrefab, _spawnPoints[spawnIndex].position, Quaternion.identity);
                            enemyInstance.BaseRoot = Instantiate(outLook.Root, enemyInstance.ReverObject);
                            enemyInstance.BaseRoot.name = outLook.Root.name;
                            _turnbasedManager.EnemyControllers.Add(enemyInstance);
                            spawnIndex++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
