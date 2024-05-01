using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEnemyCheck : MonoBehaviour
{
    //적 관련
    public List<GameObject> enemiesInRange = new List<GameObject>();

    // 트리거 적 추가
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            enemiesInRange.Add(other.gameObject);
        }
    }

    // 트리거 적 삭제
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);
        }
    }
}
