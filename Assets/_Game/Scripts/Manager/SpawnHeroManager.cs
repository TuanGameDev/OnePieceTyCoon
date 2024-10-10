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
        public Transform[] SpawnPoints;

        public TurnBasedManager TurnBasedManager;
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
                    if (spawnCount >= SpawnPoints.Length)
                    {
                        return; 
                    }

                    HeroDataSO tempHeroDataSO = ScriptableObject.CreateInstance<HeroDataSO>();
                    tempHeroDataSO.HeroID = heroData.HeroID;
                    tempHeroDataSO.HeroName = heroData.HeroName;
                    tempHeroDataSO.HeroAvatar = heroData.HeroAvatar;
                    tempHeroDataSO.IconPath = heroData.IconPath;
                    tempHeroDataSO.CharacterStat = heroData.CharacterStat;
                    tempHeroDataSO.Rarity = heroData.Rarity;
                    tempHeroDataSO.CharacterName = heroData.CharacterName;

                    HeroController heroInstance = Instantiate(_heroPrefab, SpawnPoints[spawnCount].position, Quaternion.identity);
                    CharacterNameAndRarity key = new CharacterNameAndRarity(tempHeroDataSO.CharacterName, tempHeroDataSO.Rarity);

                    if (HeroManager.Instance.CharOutLook.CharOut.TryGetValue(key, out OutLook outLook))
                    {
                        if (outLook.Root != null)
                        {
                            heroInstance.BaseRoot = Instantiate(outLook.Root, heroInstance.ReverObject);
                            heroInstance.BaseRoot.name = outLook.Root.name;
                        }
                    }
                    heroInstance.SetHeroData(tempHeroDataSO);
                    TurnBasedManager.HeroControllers.Add(heroInstance);
                    spawnCount++;
                }
            }
        }
    }
}
