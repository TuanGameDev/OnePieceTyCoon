using _Game.Scripts.Character.Hero;
using _Game.Scripts.Helper;
using _Game.Scripts.Non_Mono;
using _Game.Scripts.Scriptable_Object;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Manager
{
    public class BossManager : Singleton<BossManager>
    {
        public BossInfo Boss;

        [SerializeField]
        private CharOutLook _charOutLook;

        [SerializeField]
        private BossController _bossCtrlPrefab;

        [SerializeField]
        private Transform _spawnPoint;

        public BossController CurrentBossCtrl;

        public void SpawnBoss(int bossIndex)
        {
            if (bossIndex < 0 || bossIndex >= Boss.Keys.Count)
            {
                return;
            }

            string bossKey = GetBossKeyAtIndex(bossIndex);
            if (string.IsNullOrEmpty(bossKey) || !Boss.TryGetValue(bossKey, out HeroDataSO heroData))
            {
                return;
            }

            Vector3 spawnPosition = _spawnPoint.position;
            CurrentBossCtrl = Instantiate(_bossCtrlPrefab, spawnPosition, Quaternion.identity);

            CurrentBossCtrl.SetHeroData(heroData);

            CharacterState key = new CharacterState(heroData.CharacterName, heroData.Rarity, heroData.Elemental);
            if (_charOutLook.CharOut.TryGetValue(key, out OutLook outLook))
            {
                if (outLook.Root != null)
                {
                    GameObject rootInstance = Instantiate(outLook.Root, CurrentBossCtrl.RevertObject);
                    rootInstance.name = outLook.Root.name;
                    CurrentBossCtrl.BaseRoot = rootInstance;
                }
            }
        }
        private string GetBossKeyAtIndex(int index)
        {
            int currentIndex = 0;
            foreach (var key in Boss.Keys)
            {
                if (currentIndex == index)
                    return key;
                currentIndex++;
            }

            return null;
        }

        [System.Serializable]
        public class BossInfo : UnitySerializedDictionary<string, HeroDataSO>
        {
        }
    }
}
