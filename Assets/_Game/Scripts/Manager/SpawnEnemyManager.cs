using _Game.Scripts.Non_Mono;
using UnityEngine;
using _Game.Scripts.Scriptable_Object;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using _Game.Scripts.Helper;
using _Game.Scripts.Character.Eenmy;

namespace _Game.Scripts.Manager
{
    public class SpawnEnemyManager : Singleton<SpawnEnemyManager>
    {
        [SerializeField]
        private EnemyController _enemyBasePrefab;

        [SerializeField]
        private CharOutLook _charOutLook;

        [SerializeField]
        private List<HeroDataSO> _enemyDataList;

        [SerializeField]
        private List<Transform> _spawnPatrolPosition = new List<Transform>();

        [SerializeField]
        private MaxEnemyAndSpawnRate _spawnRate = new MaxEnemyAndSpawnRate();

        [SerializeField]
        private int _currentEnemyInZone = 0;

        [SerializeField]
        private int _sizePool = 0;

        private ObjectPool<EnemyController> _enemyPool;

        private UniTask _spawnTask;

        private void Awake()
        {
            _enemyPool = new ObjectPool<EnemyController>(_enemyBasePrefab, _sizePool, transform);
        }

        private void Start()
        {
            _spawnTask = SpawnEnemies();
        }

        private async UniTask SpawnEnemies()
        {
            while (true)
            {
                if (_currentEnemyInZone < _spawnRate.MaxEnemy)
                {
                    SpawnEnemy();
                }
                await UniTask.Delay((int)(_spawnRate.SpawnRate * 1000));
            }
        }

        private void SpawnEnemy()
        {
            var enemyData = _enemyDataList[Random.Range(0, _enemyDataList.Count)];
            CharacterState key = new CharacterState(enemyData.CharacterName, enemyData.Rarity, enemyData.Elemental);

            if (_charOutLook.CharOut.TryGetValue(key, out OutLook outLook))
            {
                Transform randomSpawnPoint = _spawnPatrolPosition[Random.Range(0, _spawnPatrolPosition.Count)];

                EnemyController enemyInstance = _enemyPool.Get();
                enemyInstance.transform.position = randomSpawnPoint.position;
                enemyInstance.transform.rotation = randomSpawnPoint.rotation;

                enemyInstance.gameObject.SetActive(true);
                enemyInstance.CurrentHP = enemyInstance.CurrentStat.Hp;
                enemyInstance.IsDead = false;
                enemyInstance.IsAttack = false;
                enemyInstance.GetComponent<BoxCollider2D>().enabled = true;

                if (outLook.Root != null)
                {
                    enemyInstance.BaseRoot = Instantiate(outLook.Root, enemyInstance.RevertObject);
                    enemyInstance.BaseRoot.name = outLook.Root.name;

                    enemyInstance.Animator = enemyInstance.BaseRoot.GetComponentInChildren<Animator>();

                    if (enemyInstance.Animator != null)
                    {
                        enemyInstance.Animator.Rebind();
                        enemyInstance.Animator.Update(0);
                    }
                }
                else
                {
                    enemyInstance.Animator = enemyInstance.RevertObject.GetComponentInChildren<Animator>();

                    if (enemyInstance.Animator != null)
                    {
                        enemyInstance.Animator.Rebind();
                        enemyInstance.Animator.Update(0);
                    }
                }

                enemyInstance.SetPatrolPoints(_spawnPatrolPosition);
                enemyInstance.SetHeroData(enemyData);

                _currentEnemyInZone++;
            }
        }

        public void ReleaseEnemy(EnemyController enemy)
        {
            _enemyPool.Release(enemy);
            _currentEnemyInZone--;
        }
    }

    [System.Serializable]
    public class MaxEnemyAndSpawnRate
    {
        public int MaxEnemy;
        public float SpawnRate;
    }
}
