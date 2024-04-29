using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMove : MonoBehaviour
{
    Transform targetVec;
    float skillAngle = 0;

    void Start()
    {
        targetVec = GameObject.Find("Target").gameObject.transform;
    }

    void Update()
    {
        skillAngle += Time.deltaTime * 100;
        transform.position += ((targetVec.position - transform.position).normalized * 0.3f * Time.deltaTime);
        transform.rotation = Quaternion.Euler(new Vector3(-90, 0, skillAngle));
    }
}
