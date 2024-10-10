using _Game.Scripts.Character.Eenmy;
using _Game.Scripts.Character.Hero;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _Game.Scripts.Manager
{
    public enum GameStat
    {
        Figth,
        Lose,
        Win
    }

    public class TurnBasedManager : MonoBehaviour
    {
        public List<HeroController> HeroControllers = new List<HeroController>();
        public List<EnemyController> EnemyControllers = new List<EnemyController>();

        public float MoveSpeed;

        [ReadOnly]
        public GameStat CurrentGameStat = GameStat.Figth;

        public TextMeshProUGUI TimerText;
        public float matchDuration;

        private Vector3[] _heroStartPositions;
        private Vector3[] _enemyStartPositions;

        void Start()
        {
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

            StartCoroutine(TurnBasedCombatRoutine());
            StartCoroutine(StartTimer());
        }

        IEnumerator TurnBasedCombatRoutine()
        {
            while (CurrentGameStat == GameStat.Figth)
            {
                yield return StartCoroutine(HeroPhase());

                if (EnemyControllers.Count > 0)
                {
                    yield return StartCoroutine(EnemyPhase());
                }
                CheckGameStatus();

                yield return new WaitForSeconds(0.5f);
            }
        }

        IEnumerator HeroPhase()
        {
            for (int i = 0; i < HeroControllers.Count; i++)
            {
                if (HeroControllers[i] != null)
                {
                    yield return StartCoroutine(HeroAttack(HeroControllers[i]));
                    yield return new WaitForSeconds(1f);
                }
            }
        }

        IEnumerator EnemyPhase()
        {
            for (int i = 0; i < EnemyControllers.Count; i++)
            {
                if (EnemyControllers[i] != null)
                {
                    yield return StartCoroutine(EnemyAttack(EnemyControllers[i]));
                    yield return new WaitForSeconds(1f);
                }
            }
        }

        IEnumerator HeroAttack(HeroController hero)
        {
            if (EnemyControllers.Count > 0)
            {
                EnemyController targetEnemy = GetRandomEnemy();
                yield return StartCoroutine(MoveToTarget(hero, targetEnemy.transform.position, true));

                yield return new WaitForSeconds(0.3f);

                hero.TryAttack();

                if (targetEnemy.IsDead)
                {
                    Destroy(targetEnemy.gameObject);
                    EnemyControllers.Remove(targetEnemy);
                }

                yield return new WaitForSeconds(0.3f);

                yield return StartCoroutine(MoveToTarget(hero, _heroStartPositions[HeroControllers.IndexOf(hero)], false));
            }
        }

        IEnumerator EnemyAttack(EnemyController enemy)
        {
            if (HeroControllers.Count > 0)
            {
                HeroController targetHero = GetRandomHero();
                yield return StartCoroutine(MoveToTarget(enemy, targetHero.transform.position, true));

                yield return new WaitForSeconds(0.3f);

                enemy.TryAttack();
                if (targetHero.IsDead)
                {
                    Destroy(targetHero.gameObject);
                    HeroControllers.Remove(targetHero);
                }

                yield return new WaitForSeconds(0.3f);

                yield return StartCoroutine(MoveToTarget(enemy, _enemyStartPositions[EnemyControllers.IndexOf(enemy)], false));
            }
        }

        void CheckGameStatus()
        {
            if (HeroControllers.Count == 0)
            {
                CurrentGameStat = GameStat.Lose;
                Time.timeScale = 0;
                Debug.Log("You Lose! All heroes are dead.");
            }
            else if (EnemyControllers.Count == 0)
            {
                CurrentGameStat = GameStat.Win;
                Time.timeScale = 0;
                Debug.Log("You Win! All enemies are defeated.");
            }
        }

        IEnumerator StartTimer()
        {
            float timeRemaining = matchDuration;

            while (timeRemaining > 0 && CurrentGameStat == GameStat.Figth)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay(timeRemaining);

                yield return null;
            }
            if (EnemyControllers.Count > 0)
            {
                CurrentGameStat = GameStat.Lose;
                Time.timeScale = 0;
            }
        }

        void UpdateTimerDisplay(float timeRemaining)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            TimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        IEnumerator MoveToTarget(MonoBehaviour controller, Vector3 target, bool movingTowardTarget)
        {
            Transform unit = controller.transform;
            Animator animator = null;
            Transform reverObject = null;

            if (controller is HeroController hero)
            {
                animator = hero.Animator;
                reverObject = hero.ReverObject;
            }
            else if (controller is EnemyController enemy)
            {
                animator = enemy.Animator;
                reverObject = enemy.ReverObject;
            }

            if (reverObject != null)
            {
                if (movingTowardTarget)
                {
                    FlipRight(reverObject);
                }
                else
                {
                    FlipLeft(reverObject);
                }
            }

            if (animator != null)
            {
                animator.SetBool("Move", true);
            }

            while (Vector3.Distance(unit.position, target) > 0.1f)
            {
                unit.position = Vector3.MoveTowards(unit.position, target, MoveSpeed * Time.deltaTime);
                yield return null;
            }

            if (animator != null)
            {
                animator.SetBool("Move", false);
                FlipRight(reverObject);
            }
        }

        void FlipRight(Transform reverObject)
        {
            Vector3 scale = reverObject.localScale;
            if (scale.x < 0)
            {
                scale.x *= -1;
            }
            reverObject.localScale = scale;
        }

        void FlipLeft(Transform reverObject)
        {
            Vector3 scale = reverObject.localScale;
            if (scale.x > 0)
            {
                scale.x *= -1;
            }
            reverObject.localScale = scale;
        }

        EnemyController GetRandomEnemy()
        {
            int randomIndex = Random.Range(0, EnemyControllers.Count);
            return EnemyControllers[randomIndex];
        }

        HeroController GetRandomHero()
        {
            int randomIndex = Random.Range(0, HeroControllers.Count);
            return HeroControllers[randomIndex];
        }
    }
}
