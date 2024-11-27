using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    public int currentHP;
    public int Def;
    public Action<int> OnAddHp;

    [Button("Add")]
    public void TakeDamage(int amount)
    {
        int finalDamage = Mathf.Max(1, amount - Def);
        currentHP -= finalDamage;
        OnAddHp?.Invoke(finalDamage);
    }
}