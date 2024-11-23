using System.Collections.Generic;
using UnityEngine;
using _Game.Scripts.Scriptable_Object;
using _Game.Scripts.Interfaces;
using _Game.Scripts.StatePatern;
using _Game.Scripts.Characters;
using System;

namespace _Game.Scripts.Character
{
    public class CharacterController : MonoBehaviour, IDamagable
    {
        [Header("State")]
        public Animator Animator;

        public Transform RevertObject;

        public GameObject BaseRoot;

        public CharacterController AttackTarget;

        public CharacterAnimations Animations;

        [Header("Attack Cooldown")]
        [SerializeField]
        private int _attackCooldown;
        private float _lastAttackTime;

        [Space(10)]
        [Header("Stats")]
        public HeroDataSO HeroDataSO;
        public CharacterStat CurrentStat;

        [Space(10)]
        public int CurrentHP;

        [Space(10)]
        public bool IsAttack = false;
        public bool IsDead = false;
        public bool FaceRight = false;

        public Action OnHealthChanged = delegate { };
        public Action OnDie;

        [Space(10)]
        public ICharacterState CurrentState;
        public LayerMask Layer;

        private List<Transform> _patrolPoints = new List<Transform>();
        protected virtual void Awake()
        {
          
        }

        protected virtual void Start()
        {
            Animator = RevertObject.GetComponentInChildren<Animator>();
            Animations = GetComponent<CharacterAnimations>();

            if (Animations != null)
            {
                Animations.SetAnimator(Animator);
            }
            SetState(new WaitingState());
            CurrentStat = HeroDataSO.CharacterStat;
            CurrentHP = CurrentStat.Hp;
        }

        protected virtual void Update()
        {
            CurrentState?.UpdateState(this);
        }

        public void SetState(ICharacterState newState)
        {
            CurrentState?.ExitState(this);
            CurrentState = newState;
            CurrentState.EnterState(this);
        }

        public virtual void TryAttack()
        {
            if (AttackTarget.IsDead)
            {
                AttackTarget = null;
                IsAttack = false;
                return;
            }

            if (AttackTarget == null || AttackTarget.gameObject == null)
            {
                AttackTarget = null;
                IsAttack = false;
                SetState(new PatrolState());
                return;
            }

            if (IsAttack && Time.time >= _lastAttackTime + _attackCooldown)
            {
                FlipTowardsTarget(AttackTarget);
                Attack();
                Animator.SetTrigger("Attack");
            }
        }

        private void Attack()
        {
            if (AttackTarget != null && IsAttack)
            {
                if (AttackTarget is IDamagable damagable)
                {
                    damagable.TakeDamage(CurrentStat.AttackDamage);
                }
                _lastAttackTime = Time.time;
            }
        }

        public void TakeDamage(int damage)
        {
            int finalDamage = Mathf.Max(1, damage - CurrentStat.Def);
            CurrentHP -= finalDamage;

            OnHealthChanged?.Invoke();

            if (CurrentHP <= 0 && !IsDead)
            {
                Die();
            }
        }

        public virtual void Die()
        {
            SetState(new DieState());
            OnDie?.Invoke();
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
        public void FlipTowardsTarget(CharacterController character)
        {
            if (character != null)
            {
                if (character.transform.position.x > transform.position.x && !FaceRight)
                {
                    FlipRight();
                }
                else if (character.transform.position.x < transform.position.x && FaceRight)
                {
                    FlipLeft();
                }
            }
        }

        public void SetPatrolPoints(List<Transform> points)
        {
            _patrolPoints = points;
        }

        public Transform GetRandomPatrolPoint()
        {
            return _patrolPoints.Count > 0 ? _patrolPoints[UnityEngine.Random.Range(0, _patrolPoints.Count)] : null;
        }

        public bool HasPatrolPoints()
        {
            return _patrolPoints.Count > 0;
        }

        public void MoveTowards(Vector3 targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, CurrentStat.MoveSpeed * Time.deltaTime);
        }
    }
}
