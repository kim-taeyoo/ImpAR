using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGyu : MonoBehaviour
{
    Vector3 direction;
    bool ismove = false;
    public GameObject spark;


    private void FixedUpdate()
    {
        if (ismove)
        {
            GetComponent<Rigidbody>().velocity = direction * 10;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit))
            {
                if (hit.collider.CompareTag("EnemyTarget"))
                {
                    Instantiate(spark, hit.point, Quaternion.identity);
                    Destroy(gameObject);
                }
            }
        }
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir;
        ismove = true;
    }
}
