using UnityEngine;

public class Test : MonoBehaviour
{
    public int _previousHp;

    public EnemyTest enemyTest;
    private void OnEnable()
    {
        enemyTest.OnAddHp += CheckBossHpChange;
    }
    private void CheckBossHpChange(int amount)
    {
        _previousHp += amount;
    }

    private void OnDestroy()
    {
        if (enemyTest != null)
        {
            enemyTest.OnAddHp -= CheckBossHpChange;
        }
    }
}