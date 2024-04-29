using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public LayerMask towerLayer;

  public void Hit(int damage)
    {
        float range = 15f;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range, towerLayer);

        foreach(var hitCollider in hitColliders)
        {
            Tower tower = hitCollider.GetComponent<Tower>();
            if(tower != null)
            {
                tower.EnemyDestroyed(gameObject);
            }
        }
    }
}
