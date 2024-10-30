using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHeroTest : MonoBehaviour
{
    public HeroCtrl HeroPrefab;
    public Transform DefaultPoint;
    [SerializeField] private List<Transform> _spawnPatrolPosition = new List<Transform>();

    void Start()
    {
        SpawnHero();
    }

    private void SpawnHero()
    {
        Vector3 spawnPosition = DefaultPoint.position;
        HeroCtrl heroInstance = Instantiate(HeroPrefab, spawnPosition, Quaternion.identity);

        // Thiết lập các điểm tuần tra cho Hero
        heroInstance.SetPatrolPoints(_spawnPatrolPosition);
    }
}
