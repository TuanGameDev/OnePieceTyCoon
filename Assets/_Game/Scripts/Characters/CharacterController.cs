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
        [Header("Stats")]
        [SerializeField]
        private SpriteRenderer _sr;

        [Header("Stats")]
        public HeroDataSO HeroDataSO;

        public CharacterStat CurrentStat;

        [Header("Attack")]
        public CharacterController AttackTarget;

        [SerializeField]
        public LayerMask layerMask;

        #region Bool

        public bool IsAttack = false;
        public bool IsDead = false;

        #endregion

        public void Start()
        {
            CurrentStat = HeroDataSO.CharacterStat;
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
            CurrentStat.Hp -= finalDamage;

            if (CurrentStat.Hp <= 0)
            {
                Die();
            }
        }

        #endregion

        public virtual void Die()
        {
            IsDead = true;
        }
    }
}
