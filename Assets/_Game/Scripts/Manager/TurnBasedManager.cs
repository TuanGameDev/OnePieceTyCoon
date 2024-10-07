using _Game.Scripts.Character.Eenmy;
using _Game.Scripts.Character.Hero;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Manager
{
    public class TurnBasedManager : MonoBehaviour
    {
        public List<HeroController> HeroControllers = new List<HeroController>();  // Danh sách hero
        public List<EnemyController> EnemyControllers = new List<EnemyController>(); // Danh sách enemy

        public float MoveSpeed = 2f; // Tốc độ di chuyển

        private Vector3[] _heroStartPositions;
        private Vector3[] _enemyStartPositions;

        public static TurnBasedManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            // Lưu lại vị trí ban đầu của tất cả hero và enemy
            _heroStartPositions = new Vector3[HeroControllers.Count];
            _enemyStartPositions = new Vector3[EnemyControllers.Count];

            for (int i = 0; i < HeroControllers.Count; i++)
            {
                _heroStartPositions[i] = HeroControllers[i].transform.position;
            }

            for (int i = 0; i < EnemyControllers.Count; i++)
            {
                _enemyStartPositions[i] = EnemyControllers[i].transform.position;
            }

            // Bắt đầu vòng lặp chiến đấu theo lượt
            StartCoroutine(TurnBasedCombatRoutine());
        }

        IEnumerator TurnBasedCombatRoutine()
        {
            int heroIndex = 0;

            while (true)
            {
                if (heroIndex < HeroControllers.Count)  // Kiểm tra hero còn lại hay không
                {
                    // Hero tấn công trước
                    yield return StartCoroutine(HeroAttack(HeroControllers[heroIndex]));
                    yield return new WaitForSeconds(1f);  // Nghỉ 1 giây giữa các lượt

                    // Enemy tấn công sau
                    if (EnemyControllers.Count > 0) // Kiểm tra có enemy không
                    {
                        yield return StartCoroutine(EnemyAttack(GetRandomEnemy()));
                        yield return new WaitForSeconds(1f);  // Nghỉ 1 giây giữa các lượt
                    }

                    heroIndex++;  // Tăng chỉ số hero lên
                }
                else
                {
                    // Reset lại chỉ số hero để lặp lại nếu cần
                    heroIndex = 0;
                }
            }
        }

        // Hàm tấn công của hero, mục tiêu ngẫu nhiên từ enemy
        // Hàm tấn công của hero
        IEnumerator HeroAttack(HeroController hero)
        {
            if (EnemyControllers.Count > 0)
            {
                EnemyController targetEnemy = GetRandomEnemy();
                yield return StartCoroutine(MoveToTarget(hero.transform, targetEnemy.transform.position));

                // Gọi phương thức Attack của hero với mục tiêu
                hero.TryAttack(); // Gọi tấn công không có tham số
                Attack(hero.name + " attacks " + targetEnemy.name); // Ghi log thông báo tấn công

                yield return new WaitForSeconds(0.5f);  // Thời gian tạm nghỉ sau khi tấn công
                yield return StartCoroutine(MoveToTarget(hero.transform, _heroStartPositions[HeroControllers.IndexOf(hero)]));
            }
        }

        // Hàm tấn công của enemy
        IEnumerator EnemyAttack(EnemyController enemy)
        {
            if (HeroControllers.Count > 0)
            {
                HeroController targetHero = GetRandomHero();
                yield return StartCoroutine(MoveToTarget(enemy.transform, targetHero.transform.position));

                // Gọi phương thức Attack của enemy với mục tiêu
                enemy.TryAttack(); // Gọi tấn công không có tham số
                Attack(enemy.name + " attacks " + targetHero.name); // Ghi log thông báo tấn công

                yield return new WaitForSeconds(0.5f);  // Thời gian tạm nghỉ sau khi tấn công
                yield return StartCoroutine(MoveToTarget(enemy.transform, _enemyStartPositions[EnemyControllers.IndexOf(enemy)]));
            }
        }


        // Hàm di chuyển đối tượng đến vị trí mục tiêu
        IEnumerator MoveToTarget(Transform unit, Vector3 target)
        {
            while (Vector3.Distance(unit.position, target) > 0.1f)
            {
                unit.position = Vector3.MoveTowards(unit.position, target, MoveSpeed * Time.deltaTime);
                yield return null;
            }
        }

        // Hàm thực hiện hành động tấn công
        void Attack(string attackMessage)
        {
            Debug.Log(attackMessage);  // Hiển thị thông báo tấn công
        }

        // Hàm lấy enemy ngẫu nhiên
        EnemyController GetRandomEnemy()
        {
            int randomIndex = Random.Range(0, EnemyControllers.Count);
            return EnemyControllers[randomIndex];
        }

        // Hàm lấy hero ngẫu nhiên
        HeroController GetRandomHero()
        {
            int randomIndex = Random.Range(0, HeroControllers.Count);
            return HeroControllers[randomIndex];
        }
    }
}
