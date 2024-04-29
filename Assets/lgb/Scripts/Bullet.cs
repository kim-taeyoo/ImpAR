using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Transform target;
    public float speed = 10f;
    public float rotateSpeed = 5f;
    public float maxRotateAngle = 30f;

    public GameObject spark;

    void Start()
    {
        target = GameObject.Find("�ѹ޴³༮").transform;
        transform.LookAt(target);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(20, 120, 0));
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        
        float dot = Vector3.Dot(transform.forward, direction);
        float rotateAngle = Mathf.Acos(dot) * Mathf.Rad2Deg * rotateSpeed * Time.deltaTime;

        rotateAngle = Mathf.Clamp(rotateAngle, -maxRotateAngle, maxRotateAngle);

        Vector3 axis = Vector3.Cross(transform.forward, direction);
        transform.rotation = Quaternion.AngleAxis(rotateAngle, axis) * transform.rotation;

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "�ѹ޴³༮")
        {
            Debug.Log("�ѹ޾Ҵ�");
            Vector3 sparkAngle = transform.rotation.eulerAngles + new Vector3(0,90,0);
            Instantiate(spark, transform.position, Quaternion.Euler(sparkAngle));
            Destroy(spark, 1);
            Destroy(gameObject);
        }
    }
}
