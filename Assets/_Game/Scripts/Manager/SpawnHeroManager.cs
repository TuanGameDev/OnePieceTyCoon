using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Game.Scripts.Character.Hero;
using _Game.Scripts.Manager;
using _Game.Scripts.Scriptable_Object;
using _Game.Scripts.Non_Mono;
using _Game.Scripts.Enums;

namespace _Game.Scripts.Manager
{
    public class SpawnHeroManager : MonoBehaviour
    {
        [SerializeField]
        private HeroController _heroPrefab;

        [SerializeField]
        private Transform[] _spawnPoints;
        private void Start()
        {
            SpawnHeroes();
        }

        private void SpawnHeroes()
        {
            if (HeroManager.Instance.HeroesReady.Count == 0)
            {
                Debug.Log("No heroes in the HeroesReady list to spawn.");
                return;
            }

            int spawnCount = 0;

            for (int i = 0; i < HeroManager.Instance.HeroesReady.Count; i++)
            {
                HeroDataList heroDataList = HeroManager.Instance.HeroesReady[i];

                foreach (HeroData heroData in heroDataList.heroes)
                {
                    if (spawnCount >= _spawnPoints.Length)
                    {
                        return; 
                    }

                    HeroDataSO tempHeroDataSO = ScriptableObject.CreateInstance<HeroDataSO>();
                    tempHeroDataSO.HeroID = heroData.HeroID;
                    tempHeroDataSO.HeroName = heroData.HeroName;
                    tempHeroDataSO.HeroAvatar = heroData.HeroAvatar;
                    tempHeroDataSO.CharacterStat = heroData.CharacterStat;
                    tempHeroDataSO.Rank = heroData.Rank;
                    tempHeroDataSO.CharacterName = heroData.CharacterName;

                    HeroController heroInstance = Instantiate(_heroPrefab, _spawnPoints[spawnCount].position, Quaternion.identity);
                    CharacterNameAndRank key = new CharacterNameAndRank(tempHeroDataSO.CharacterName, tempHeroDataSO.Rank);

                    if (HeroManager.Instance.CharOutLook.CharOut.TryGetValue(key, out OutLook outLook))
                    {
                        if (outLook.Root != null)
                        {
                            heroInstance.BaseRoot = Instantiate(outLook.Root, heroInstance.ReverObject);
                            heroInstance.BaseRoot.name = outLook.Root.name;
                        }
                    }
                    heroInstance.SetHeroData(tempHeroDataSO);
                    spawnCount++;
                }
            }
        }
    }
}
