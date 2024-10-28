using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Game.Scripts.Character.Hero;
using _Game.Scripts.Manager;
using _Game.Scripts.Scriptable_Object;
using _Game.Scripts.Non_Mono;
using _Game.Scripts.Enums;
using Sirenix.OdinInspector;
using _Game.Scripts.Helper;

namespace _Game.Scripts.Manager
{
    public class SpawnHeroManager : MonoBehaviour
    {
        [SerializeField]
        private HeroController _heroPrefab;

        [SerializeField]
        private CharOutLook CharOutLook;

        [SerializeField]
        private Transform DefaultPoint;


        [SerializeField]
        private ObjectPool<HeroController> _heroPool;
        private void Awake()
        {
            _heroPool = new ObjectPool<HeroController>(_heroPrefab, 6, transform);
        }

        private void SpawnHeroes()
        {
            if (HeroManager.Instance.HeroesAvailable.Count == 0)
            {
                Debug.Log("No heroes in the HeroesReady list to spawn.");
                return;
            }

            foreach (var heroDataList in HeroManager.Instance.HeroesAvailable)
            {
                foreach (HeroData heroData in heroDataList.heroes)
                {
                    Vector3 spawnPosition = DefaultPoint.position;

                    HeroController heroInstance = _heroPool.Get();
                    heroInstance.transform.position = spawnPosition;

                    HeroDataSO tempHeroDataSO = ScriptableObject.CreateInstance<HeroDataSO>();
                    tempHeroDataSO.HeroID = heroData.HeroID;
                    tempHeroDataSO.HeroName = heroData.HeroName;
                    tempHeroDataSO.HeroAvatar = heroData.HeroAvatar;
                    tempHeroDataSO.CharacterStat = heroData.CharacterStat;
                    tempHeroDataSO.Rank = heroData.Rank;
                    tempHeroDataSO.CharacterName = heroData.CharacterName;

                    CharacterNameAndRank key = new CharacterNameAndRank(tempHeroDataSO.CharacterName, tempHeroDataSO.Rank);

                    if (CharOutLook.CharOut.TryGetValue(key, out OutLook outLook))
                    {
                        if (outLook.Root != null)
                        {
                            heroInstance.BaseRoot = Instantiate(outLook.Root, heroInstance.ReverObject);
                            heroInstance.BaseRoot.name = outLook.Root.name;
                        }
                    }
                    heroInstance.SetHeroData(tempHeroDataSO);
                }
            }
        }
    }
}
