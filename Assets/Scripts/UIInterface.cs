using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInterface : MonoBehaviour
{
    public GameObject tower;
    public GameObject select;

    GameObject focusObs;

    private bool turretSpawn = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!RaycastWithoutTriggers(ray, out hit)) return;
            focusObs = Instantiate(select, hit.point, select.transform.rotation);
            DisableColliders();

            Renderer rend = focusObs.GetComponent<Renderer>();
            if (!hit.collider.gameObject.CompareTag("platform"))
            {
                Color newColor = new Color(238f / 255f, 74f / 255f, 88f / 255f);
                rend.material.color = newColor;
            }
            else
            {
                Color newColor = new Color(107f / 255f, 249f / 255f, 121f / 255f);
                rend.material.color = newColor;
            }
        }
        else if (Input.GetMouseButton(0) && focusObs != null)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!RaycastWithoutTriggers(ray, out hit)) return;


            GameObject hitObject = hit.collider.gameObject;
            if (!hitObject.CompareTag("platform") && !hitObject.CompareTag("Occupied"))
            {
                // hit�� ������Ʈ�� �ٴ� �κ��� Ȯ���ϱ� ���� ����ĳ��Ʈ
                RaycastHit floorHit;
                // ���̸� hit�� ������Ʈ�� �ٷ� �Ʒ��� �߻� (���� ���, y������ -1)
                Ray downRay = new Ray(hitObject.transform.position, -Vector3.up);
                if (Physics.Raycast(downRay, out floorHit))
                {
                    Renderer rend = focusObs.GetComponent<Renderer>();
                    // �ٴ� ������Ʈ�� 'platform' �Ǵ� 'Occupied' �±׸� ������ �ִ��� Ȯ��
                    if (floorHit.collider.gameObject.CompareTag("platform"))
                    {
                        focusObs.transform.position = hit.collider.gameObject.transform.position;
                        Color newColor = new Color(238f / 255f, 74f / 255f, 88f / 255f);
                        rend.material.color = newColor;
                    }
                    if (floorHit.collider.gameObject.CompareTag("Occupied"))
                    {
                        focusObs.transform.position = hit.collider.gameObject.transform.position;
                        Color newColor = new Color(107f / 255f, 249f / 255f, 121f / 255f);
                        rend.material.color = newColor;
                    }
                }
            }
            else
            {
                // hit�� ������Ʈ�� 'platform' �Ǵ� 'Occupied'�� ����� ó��
                focusObs.transform.position = hit.collider.gameObject.transform.position;

                Renderer rend = focusObs.GetComponent<Renderer>();
                if (!hit.collider.gameObject.CompareTag("platform"))
                {
                    Color newColor = new Color(238f / 255f, 74f / 255f, 88f / 255f);
                    rend.material.color = newColor;
                }
                else
                {
                    Color newColor = new Color(107f / 255f, 249f / 255f, 121f / 255f);
                    rend.material.color = newColor;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0) && focusObs != null)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!RaycastWithoutTriggers(ray, out hit)) return;
            if (hit.collider.gameObject.CompareTag("platform") && hit.normal.Equals(new Vector3(0, 1, 0)))
            {
                Destroy(focusObs);
                focusObs = Instantiate(tower, hit.point, tower.transform.rotation);

                hit.collider.gameObject.tag = "Occupied";
                focusObs.transform.position = new Vector3(hit.collider.gameObject.transform.position.x, focusObs.transform.position.y + 20, hit.collider.gameObject.transform.position.z);
                EnableColliders();

                //������Ʈ �߰�
                Rigidbody rb = focusObs.AddComponent<Rigidbody>();
                // Rigidbody�� interpolation �Ӽ��� Extrapolate�� ����
                rb.interpolation = RigidbodyInterpolation.Extrapolate;
                // Rigidbody�� collision detection �Ӽ��� Continuous�� ����
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

                BoxCollider boxCollider = focusObs.AddComponent<BoxCollider>();
                boxCollider.size = new Vector3(0.08f, 0.1f, 0.08f); // �ݶ��̴��� ũ�� ����
                boxCollider.center = new Vector3(0f, 0.05f, 0f); // �ݶ��̴��� �߽� ��ġ ����
            }
            else
            {
                Destroy(focusObs);
            }
            focusObs = null;
        }

       /* if (turretSpawn)
        {
            RaycastHit hit;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (!RaycastWithoutTriggers(ray, out hit)) return;
            focusObs = Instantiate(select, hit.point, select.transform.rotation);
            DisableColliders();

            GameObject hitObject = hit.collider.gameObject;
            if (!hitObject.CompareTag("platform") && !hitObject.CompareTag("Occupied"))
            {
                // hit�� ������Ʈ�� �ٴ� �κ��� Ȯ���ϱ� ���� ����ĳ��Ʈ
                RaycastHit floorHit;
                // ���̸� hit�� ������Ʈ�� �ٷ� �Ʒ��� �߻� (���� ���, y������ -1)
                Ray downRay = new Ray(hitObject.transform.position, -Vector3.up);
                if (Physics.Raycast(downRay, out floorHit))
                {
                    Renderer rend = focusObs.GetComponent<Renderer>();
                    // �ٴ� ������Ʈ�� 'platform' �Ǵ� 'Occupied' �±׸� ������ �ִ��� Ȯ��
                    if (floorHit.collider.gameObject.CompareTag("platform"))
                    {
                        focusObs.transform.position = hit.collider.gameObject.transform.position;
                        Color newColor = new Color(238f / 255f, 74f / 255f, 88f / 255f);
                        rend.material.color = newColor;
                    }
                    if (floorHit.collider.gameObject.CompareTag("Occupied"))
                    {
                        focusObs.transform.position = hit.collider.gameObject.transform.position;
                        Color newColor = new Color(107f / 255f, 249f / 255f, 121f / 255f);
                        rend.material.color = newColor;
                    }
                }
            }
            else
            {
                // hit�� ������Ʈ�� 'platform' �Ǵ� 'Occupied'�� ����� ó��
                focusObs.transform.position = hit.collider.gameObject.transform.position;

                Renderer rend = focusObs.GetComponent<Renderer>();
                if (!hit.collider.gameObject.CompareTag("platform"))
                {
                    Color newColor = new Color(238f / 255f, 74f / 255f, 88f / 255f);
                    rend.material.color = newColor;
                }
                else
                {
                    Color newColor = new Color(107f / 255f, 249f / 255f, 121f / 255f);
                    rend.material.color = newColor;
                }
            }
        }
        *//*if(Input .GetMouseButton(0) && focusObs != null)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input .mousePosition);
            if (!RaycastWithoutTriggers(ray, out hit)) return;


            
        }*//*
        if (Input.GetMouseButtonDown(0) && focusObs != null)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!RaycastWithoutTriggers(ray, out hit)) return;
            if (hit.collider.gameObject.CompareTag("platform") && hit.normal.Equals(new Vector3(0, 1, 0)))
            {
                Destroy(focusObs);
                focusObs = Instantiate(tower, hit.point, tower.transform.rotation);

                hit.collider.gameObject.tag = "Occupied";
                focusObs.transform.position = new Vector3(hit.collider.gameObject.transform.position.x, focusObs.transform.position.y + 20, hit.collider.gameObject.transform.position.z);
                EnableColliders();

                //������Ʈ �߰�
                Rigidbody rb = focusObs.AddComponent<Rigidbody>();
                // Rigidbody�� interpolation �Ӽ��� Extrapolate�� ����
                rb.interpolation = RigidbodyInterpolation.Extrapolate;
                // Rigidbody�� collision detection �Ӽ��� Continuous�� ����
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

                BoxCollider boxCollider = focusObs.AddComponent<BoxCollider>();
                boxCollider.size = new Vector3(0.08f, 0.1f, 0.08f); // �ݶ��̴��� ũ�� ����
                boxCollider.center = new Vector3(0f, 0.05f, 0f); // �ݶ��̴��� �߽� ��ġ ����

                turretSpawn = false;
            }
            else
            {
                Destroy(focusObs);
            }
            focusObs = null;
        }*/
    }

    private void DisableColliders()
    {
        SetCollidersEnable(false);
    }
    private void EnableColliders()
    {
        SetCollidersEnable(true);
    }

    private void SetCollidersEnable(bool v)
    {
        Collider[] childColliders = focusObs.GetComponentsInChildren<Collider>(true);
        Collider[] mainColliders = focusObs.GetComponents<Collider>();

        foreach (Collider collider in childColliders)
        {
            collider.enabled = v;
        }
        foreach (Collider collider in mainColliders)
        {
            collider.enabled = v;
        }
    }

    private bool RaycastWithoutTriggers(Ray ray, out RaycastHit hit)
    {
        RaycastHit[] hits = Physics.RaycastAll(ray);

        Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

        foreach (RaycastHit raycastHit in hits)
        {
            if (!raycastHit.collider.isTrigger)
            {
                hit = raycastHit;
                return true;
            }
        }

        hit = new RaycastHit();
        return false;
    }

    public void TurretStart()
    {
        turretSpawn = !turretSpawn;
    }
}
