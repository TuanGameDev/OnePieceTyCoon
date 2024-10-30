using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Game.Scripts.Scriptable_Object;
using _Game.Scripts.Interfaces;
using _Game.Scripts.Characters;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks;

namespace _Game.Scripts.Character
{
    public class CharacterController : MonoBehaviour, IDamagable
    {
        [Header("State")]
        public Animator Animator;

        public Transform RevertObject;

        [SerializeField, ReadOnly]
        private Transform _targetIndex;

        private List<Transform> _patrolPoints = new List<Transform>();

        public GameObject BaseRoot;

        [Header("Attack")]
        public CharacterController AttackTarget;

        [SerializeField]
        public LayerMask layerMask;

        [Header("Stats")]
        public HeroDataSO HeroDataSO;

        public CharacterStat CurrentStat;

        public int CurrentHP;

        #region Bool

        public bool IsAttack = false;
        public bool IsDead = false;
        public bool FaceRight = false;

        #endregion

        public void Start()
        {
            Animator = FindObjectOfType<Animator>();
            CurrentStat = HeroDataSO.CharacterStat;
            CurrentHP = CurrentStat.Hp;
        }

        #region AttackFusion

        public void TryAttack()
        {
            Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, 1f, layerMask);

            if (targets.Length > 0)
            {
                AttackTarget = targets[0].GetComponent<CharacterController>();

                if (AttackTarget != null)
                {
                    IsAttack = true;
                    Attack();
                }
            }
            else
            {
                AttackTarget = null;
                IsAttack = false;
            }
        }

        public virtual void Attack()
        {
            if (AttackTarget != null && IsAttack)
            {
                if (AttackTarget is IDamagable damagable)
                {
                    damagable.TakeDamage(CurrentStat.AttackDamage);
                    Animator.SetTrigger("Attack");
                }
            }
        }

        public virtual void TakeDamage(int damage)
        {
            int finalDamage;

            if (CurrentStat.Def >= damage)
            {
                finalDamage = 1;
            }
            else
            {
                finalDamage = damage - CurrentStat.Def;
            }
            CurrentHP -= finalDamage;

            if (CurrentHP <= 0)
            {
                Die();
            }
        }

        #endregion

        public virtual void Die()
        {
            IsDead = true;
            Destroy(gameObject);
        }

        #region Patrol

        public void SetPatrolPoints(List<Transform> points)
        {
            _patrolPoints = points;
            if (_patrolPoints.Count > 0)
            {
                StartCoroutine(PatrolRoutineAsync());
            }
        }

        private IEnumerator PatrolRoutineAsync()
        {
            while (true)
            {
                _targetIndex = _patrolPoints[Random.Range(0, _patrolPoints.Count)];

                if (_targetIndex.position.x > transform.position.x && !FaceRight)
                {
                    FlipRight();
                }
                else if (_targetIndex.position.x < transform.position.x && FaceRight)
                {
                    FlipLeft();
                }
                if(Animator != null)
                {
                    Animator.SetBool("Move", true);
                }
                while (Vector3.Distance(transform.position, _targetIndex.position) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, _targetIndex.position,CurrentStat.MoveSpeed * Time.deltaTime);
                    yield return null;
                }
                if (Animator != null)
                {
                    Animator.SetBool("Move", false);
                }
                yield return new WaitForSeconds(2f);
            }
        }

        public void FlipRight()
        {
            if (!FaceRight)
            {
                FaceRight = true;
                Vector3 theScale = RevertObject.localScale;
                theScale.x = Mathf.Abs(theScale.x);
                RevertObject.localScale = theScale;
            }
        }

        public void FlipLeft()
        {
            if (FaceRight)
            {
                FaceRight = false;
                Vector3 theScale = RevertObject.localScale;
                theScale.x = -Mathf.Abs(theScale.x);
                RevertObject.localScale = theScale;
            }
        }
        #endregion
    }
}
