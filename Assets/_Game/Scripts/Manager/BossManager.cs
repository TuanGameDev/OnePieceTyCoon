using _Game.Scripts.Character;
using _Game.Scripts.Character.Hero;
using _Game.Scripts.Helper;
using _Game.Scripts.Non_Mono;
using _Game.Scripts.Scriptable_Object;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Manager
{
    public class BossManager : Singleton<BossManager>
    {
        [Title("Boss Information List")]
        public List<BossInfo> BossInfos;

        [SerializeField, Title("Dependencies")]
        private CharOutLook _charOutLook;

        [SerializeField]
        private BossController _bossCtrlPrefab;

        [SerializeField]
        private Transform _spawnPoint;

        [SerializeField]
        private List<Transform> _spawnPatrolPosition;

        public BossController CurrentBossCtrl;

        public int PreviousScore;

        public void SpawnBoss(int bossIndex)
        {
            Debug.Log("SpawnBoss called with index: " + bossIndex);

            if (BossInfos == null || BossInfos.Count == 0)
            {
                Debug.LogError("BossInfos is null or empty!");
                return;
            }

            if (bossIndex < 0 || bossIndex >= BossInfos.Count)
            {
                Debug.LogError("Boss index is out of range!");
                return;
            }

            Vector3 spawnPosition = _spawnPoint.position;
            CurrentBossCtrl = Instantiate(_bossCtrlPrefab, spawnPosition, Quaternion.identity);
            Debug.Log("Boss instantiated successfully.");

            // Rest of the logic

            CurrentBossCtrl.SetHeroData(BossInfos[0].HeroData);

            CurrentBossCtrl.OnDamaggeChanged += CheckBossHpChange;
            PreviousScore = 0;
            CharacterState key = new CharacterState(BossInfos[0].HeroData.HeroName, BossInfos[0].HeroData.Rarity, BossInfos[0].HeroData.Elemental);
            if (_charOutLook != null && _charOutLook.CharOut.TryGetValue(key, out OutLook outLook))
            {
                if (outLook.Root != null)
                {
                    GameObject rootInstance = Instantiate(outLook.Root, CurrentBossCtrl.RevertObject);
                    rootInstance.name = outLook.Root.name;

                    CurrentBossCtrl.BaseRoot = rootInstance;

                    CurrentBossCtrl.SetPatrolPoints(_spawnPatrolPosition);

                    CurrentBossCtrl.SetState(new WaitingState());

                    CurrentBossCtrl.Animator = CurrentBossCtrl.RevertObject.GetComponentInChildren<Animator>();
                    if (CurrentBossCtrl.Animator != null)
                    {
                        CurrentBossCtrl.Animator.Rebind();
                        CurrentBossCtrl.Animator.Update(0);

                    }
                }
            }
        }

        private void CheckBossHpChange(int damage)
        {
            PreviousScore += damage;
        }

        private void OnDestroy()
        {
            if (CurrentBossCtrl != null)
            {
                CurrentBossCtrl.OnDamaggeChanged -= CheckBossHpChange;
                Destroy(CurrentBossCtrl.gameObject);
                CurrentBossCtrl = null;
            }
        }

        [System.Serializable]
        public class BossInfo
        {
            [Tooltip("Hero data for this boss")]
            public HeroDataSO HeroData;
        }
    }
}
