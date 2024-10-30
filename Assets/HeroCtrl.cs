using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class HeroCtrl : MonoBehaviour
{
    public float MoveSpeed = 3f;

    [SerializeField, ReadOnly]
    private Transform _targetIndex;

    private List<Transform> _patrolPoints = new List<Transform>();

    // Phương thức để thiết lập các điểm tuần tra
    public void SetPatrolPoints(List<Transform> points)
    {
        _patrolPoints = points;
        if (_patrolPoints.Count > 0)
        {
            StartCoroutine(Patrol());
        }
    }

    // Coroutine di chuyển tuần tra giữa các điểm
    private IEnumerator Patrol()
    {
        while (true)
        {
            // Chọn một điểm ngẫu nhiên từ danh sách _patrolPoints
            _targetIndex = _patrolPoints[Random.Range(0, _patrolPoints.Count)];

            // Di chuyển đến điểm mục tiêu
            while (Vector3.Distance(transform.position, _targetIndex.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, _targetIndex.position, MoveSpeed * Time.deltaTime);
                yield return null;
            }

            // Đợi một khoảng thời gian trước khi di chuyển đến điểm tiếp theo
            yield return new WaitForSeconds(2f);
        }
    }
}
