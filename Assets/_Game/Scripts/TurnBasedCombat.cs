using UnityEngine;
using System.Collections;

public class TurnBasedCombatRandom : MonoBehaviour
{
    public Transform[] heroes;   // Mảng chứa các hero
    public Transform[] enemies;  // Mảng chứa các enemy
    public float moveSpeed = 2f; // Tốc độ di chuyển

    private Vector3[] heroStartPositions;   // Vị trí ban đầu của các hero
    private Vector3[] enemyStartPositions;  // Vị trí ban đầu của các enemy

    void Start()
    {
        // Lưu lại vị trí ban đầu của tất cả hero và enemy
        heroStartPositions = new Vector3[heroes.Length];
        enemyStartPositions = new Vector3[enemies.Length];

        for (int i = 0; i < heroes.Length; i++)
        {
            heroStartPositions[i] = heroes[i].position;
        }

        for (int i = 0; i < enemies.Length; i++)
        {
            enemyStartPositions[i] = enemies[i].position;
        }

        // Bắt đầu vòng lặp chiến đấu theo lượt
        StartCoroutine(TurnBasedCombatRoutine());
    }

    IEnumerator TurnBasedCombatRoutine()
    {
        while (true)
        {
            // Xen kẽ hero và enemy đánh nhau
            for (int i = 0; i < heroes.Length; i++)
            {
                // Hero tấn công trước
                yield return StartCoroutine(HeroAttack(heroes[i]));
                yield return new WaitForSeconds(1f);  // Nghỉ 1 giây giữa các lượt

                // Enemy tấn công sau
                if (i < enemies.Length) // Kiểm tra để tránh lỗi nếu số lượng enemy ít hơn hero
                {
                    yield return StartCoroutine(EnemyAttack(enemies[i]));
                    yield return new WaitForSeconds(1f);  // Nghỉ 1 giây giữa các lượt
                }
            }
        }
    }

    // Hàm tấn công của hero, mục tiêu ngẫu nhiên từ enemy
    IEnumerator HeroAttack(Transform hero)
    {
        if (enemies.Length > 0)
        {
            Transform targetEnemy = GetRandomEnemy();
            yield return StartCoroutine(MoveToTarget(hero, targetEnemy.position));
            Attack(hero.name + " attacks " + targetEnemy.name);
            yield return new WaitForSeconds(0.5f);  // Thời gian tạm nghỉ sau khi tấn công
            yield return StartCoroutine(MoveToTarget(hero, heroStartPositions[System.Array.IndexOf(heroes, hero)]));
        }
    }

    // Hàm tấn công của enemy, mục tiêu ngẫu nhiên từ hero
    IEnumerator EnemyAttack(Transform enemy)
    {
        if (heroes.Length > 0)
        {
            Transform targetHero = GetRandomHero();
            yield return StartCoroutine(MoveToTarget(enemy, targetHero.position));
            Attack(enemy.name + " attacks " + targetHero.name);
            yield return new WaitForSeconds(0.5f);  // Thời gian tạm nghỉ sau khi tấn công
            yield return StartCoroutine(MoveToTarget(enemy, enemyStartPositions[System.Array.IndexOf(enemies, enemy)]));
        }
    }

    // Hàm di chuyển đối tượng đến vị trí mục tiêu
    IEnumerator MoveToTarget(Transform unit, Vector3 target)
    {
        while (Vector3.Distance(unit.position, target) > 0.1f)
        {
            unit.position = Vector3.MoveTowards(unit.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    // Hàm thực hiện hành động tấn công
    void Attack(string attackMessage)
    {
        Debug.Log(attackMessage);
    }

    // Hàm lấy enemy ngẫu nhiên
    Transform GetRandomEnemy()
    {
        int randomIndex = Random.Range(0, enemies.Length);
        return enemies[randomIndex];
    }

    // Hàm lấy hero ngẫu nhiên
    Transform GetRandomHero()
    {
        int randomIndex = Random.Range(0, heroes.Length);
        return heroes[randomIndex];
    }
}
