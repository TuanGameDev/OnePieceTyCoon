using System.Collections.Generic;
using UnityEngine;
using _Game.Scripts.Character.Hero;
using _Game.Scripts.Scriptable_Object;
using _Game.Scripts.Non_Mono;

namespace _Game.Scripts.Manager
{
    public class SpawnHeroManager : MonoBehaviour
    {
        [SerializeField] private HeroController _heroPrefab;

        [SerializeField] private CharOutLook _charOutLook;

        [SerializeField] private Transform _defaultPoint;

        [SerializeField]
        private List<Transform> _spawnPatrolPosition = new List<Transform>();

        private Dictionary<int, HeroController> _spawnedHeroes = new Dictionary<int, HeroController>();

        public static SpawnHeroManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        public void SpawnHero(HeroData heroData)
        {
            if (!_spawnedHeroes.ContainsKey(heroData.HeroID))
            {
                Vector3 spawnPosition = _defaultPoint.position;
                HeroController heroInstance = Instantiate(_heroPrefab, spawnPosition, Quaternion.identity);

                HeroDataSO tempHeroDataSO = ScriptableObject.CreateInstance<HeroDataSO>();
                tempHeroDataSO.HeroID = heroData.HeroID;
                tempHeroDataSO.HeroName = heroData.HeroName;
                tempHeroDataSO.HeroAvatar = heroData.HeroAvatar;
                tempHeroDataSO.IconAvatar = heroData.IconAvatar;
                tempHeroDataSO.CharacterStat = heroData.CharacterStat;
                tempHeroDataSO.Rank = heroData.Rank;
                tempHeroDataSO.Power = heroData.Power;
                tempHeroDataSO.CharacterName = heroData.CharacterName;

                heroInstance.SetPatrolPoints(_spawnPatrolPosition);

                CharacterNameAndRank key = new CharacterNameAndRank(tempHeroDataSO.CharacterName, tempHeroDataSO.Rank);
                if (_charOutLook.CharOut.TryGetValue(key, out OutLook outLook))
                {
                    if (outLook.Root != null)
                    {
                        heroInstance.BaseRoot = Instantiate(outLook.Root, heroInstance.RevertObject);
                        heroInstance.BaseRoot.name = outLook.Root.name;

                        heroInstance.Animator.Update(1);
                        heroInstance.Animator.Rebind();
                    }
                }
                heroInstance.SetHeroData(tempHeroDataSO);
                _spawnedHeroes[heroData.HeroID] = heroInstance;
            }
        }

        public HeroController GetSpawnedHero(int heroID)
        {
            _spawnedHeroes.TryGetValue(heroID, out var hero);
            return hero;
        }

        public void RemoveHero(int heroID)
        {
            if (_spawnedHeroes.TryGetValue(heroID, out var heroInstance))
            {
                Destroy(heroInstance.gameObject);
                _spawnedHeroes.Remove(heroID);
            }
        }
    }
}
