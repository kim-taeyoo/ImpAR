using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : AbstractProjectile
{
    private TrailRenderer trailRenderer;

    private void Awake()
    {
     /*   trailRenderer = GetComponent<TrailRenderer>();
        if(trailRenderer == null)
        {
            return;
        }

        trailRenderer.time = 0.5f;
        trailRenderer.startWidth = 0.1f;
        trailRenderer.endWidth = 0.0f;*/
    }
    void Start()
    {
        Destroy(gameObject, 3f); // 3�� �Ŀ� �ڵ����� ������Ÿ�� �ı�
    }
}
