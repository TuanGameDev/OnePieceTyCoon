using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Game.Scripts.Scriptable_Object;
using _Game.Scripts.Interfaces;
using _Game.Scripts.Characters;

namespace _Game.Scripts.Character
{
    public class CharacterController : MonoBehaviour, IDamagable
    {
        public Animator Animator;

        public Transform ReverObject;

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
            Animator = GetComponentInChildren<Animator>();
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
    }
}
