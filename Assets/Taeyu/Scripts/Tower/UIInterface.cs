using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInterface : MonoBehaviour
{
    //�����յ�
    public GameObject turret;
    public GameObject wizzardTower;
    public GameObject select;

    //��ġ�� ������Ʈ��, ����ǥ�� ������Ʈ
    GameObject focusObs;
    GameObject selectObj;

    //��ư �ִ� canvas ��������
    public GameObject canvas;

    private bool turretSpawn = false;

    //��ġ�ɶ� ����Ʈ
    public ParticleSystem particlePrefab;
    public ParticleSystem particlePrefab2;

    public AudioClip spawnSound;

    //Ÿ�� ����
    enum TowerType { Turret, WizzardTower };
    TowerType curType;

    void Update()
    {
        if (turretSpawn)
        {
            RaycastHit hit;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (!RaycastWithoutTriggers(ray, out hit)) return;

            GameObject hitObject = hit.collider.gameObject;
            //ray�� ���� �κ��� �÷����̳� �̹� ��ġ�� �÷����� �ƴҶ�(�ٸ� ������Ʈ�϶�)
            if (!hitObject.CompareTag("platform") && !hitObject.CompareTag("Occupied"))
            {
                // ���̸� hit�� ������Ʈ�� �ٷ� �Ʒ��� �߻�
                Ray downRay = new Ray(hitObject.transform.position, -Vector3.up);
                RaycastHit[] hits = Physics.RaycastAll(downRay, Mathf.Infinity);
                //�Ʒ��� �� ray�� ���� ��� ������Ʈ�� �˻�
                foreach (var floorHit in hits)
                {
                    // 'platform' �Ǵ� 'Occupied' �±׸� ���� ������Ʈ�� ������
                    if (floorHit.collider.gameObject.CompareTag("platform") || floorHit.collider.gameObject.CompareTag("Occupied"))
                    {
                        Renderer rend = selectObj.GetComponent<Renderer>();
                        // ��ġ���� ������Ʈ�� �ش� ������Ʈ�� ��ġ�� �̵�
                        selectObj.transform.position = hit.collider.gameObject.transform.position + new Vector3(0, 0.005f, 0);

                        //�÷��� ������Ʈ�� �θ� plane �̴ϱ� �ش� ������Ʈ�� ȸ���� ����
                        if (floorHit.collider.gameObject.transform.parent != null)
                        {
                            selectObj.transform.rotation = floorHit.collider.gameObject.transform.parent.rotation;
                        }

                        //�ͷ��϶� �������� üũ(���󺯰�)
                        if (curType == TowerType.Turret)
                        {
                            Color newColor = floorHit.collider.gameObject.CompareTag("platform") ?
                            new Color(107f / 255f, 249f / 255f, 121f / 255f) : // platform�� ���
                            new Color(238f / 255f, 74f / 255f, 88f / 255f);  // Occupied�� ���
                            rend.material.color = newColor;
                            break;
                        }
                        //���ڵ� Ÿ���϶� �������� üũ
                        else if (curType == TowerType.WizzardTower)
                        {
                            //platform�϶�
                            if (floorHit.collider.gameObject.CompareTag("platform"))
                            {
                                if (CheckSurroundingBlocks("platform", hit, 0.045f))
                                {
                                    Color newColor = new Color(107f / 255f, 249f / 255f, 121f / 255f);
                                    rend.material.color = newColor;
                                }
                                else
                                {
                                    Color newColor = new Color(238f / 255f, 74f / 255f, 88f / 255f);
                                    rend.material.color = newColor;
                                }
                            }
                            //Ocuppied�϶�
                            else if (floorHit.collider.gameObject.CompareTag("Occupied"))
                            {
                                Color newColor = new Color(238f / 255f, 74f / 255f, 88f / 255f);
                                rend.material.color = newColor;
                            }
                        }
                    }
                }
            }
            else
            {
                // hit�� ������Ʈ�� 'platform' �Ǵ� 'Occupied'�� ����� ó��
                selectObj.transform.position = hit.collider.gameObject.transform.position + new Vector3(0, 0.005f, 0);

                if (hit.collider.gameObject.transform.parent != null)
                {
                    selectObj.transform.rotation = hit.collider.gameObject.transform.parent.rotation;
                }

                Renderer rend = selectObj.GetComponent<Renderer>();

                //�ͷ��϶� �������� üũ
                if (curType == TowerType.Turret)
                {
                    if (hit.collider.gameObject.CompareTag("platform"))
                    {
                        Color newColor = new Color(107f / 255f, 249f / 255f, 121f / 255f);
                        rend.material.color = newColor;
                    }
                    else
                    {
                        Color newColor = new Color(238f / 255f, 74f / 255f, 88f / 255f);
                        rend.material.color = newColor;
                    }
                }
                //���ڵ� Ÿ���϶� �������� üũ
                else if (curType == TowerType.WizzardTower)
                {
                    if (hit.collider.gameObject.CompareTag("platform"))
                    {
                        if (CheckSurroundingBlocks("platform", hit, 0.045f))
                        {
                            Color newColor = new Color(107f / 255f, 249f / 255f, 121f / 255f);
                            rend.material.color = newColor;
                        }
                        else
                        {
                            Color newColor = new Color(238f / 255f, 74f / 255f, 88f / 255f);
                            rend.material.color = newColor;
                        }
                    }
                    else
                    {
                        Color newColor = new Color(238f / 255f, 74f / 255f, 88f / 255f);
                        rend.material.color = newColor;
                    }
                }
            }
        }
    }

    IEnumerator MoveObjectToPosition(GameObject obj, Vector3 targetPosition, float duration, ParticleSystem particleSystemInstance, ParticleSystem particleSystemInstance2)
    {
        float time = 0;
        Vector3 startPosition = obj.transform.position;

        while (time < duration)
        {
            obj.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        obj.transform.position = targetPosition; // ���� ��ġ ����

        // ��� �̵��� �Ϸ�� �� Rigidbody�� BoxCollider �߰�
        /* Rigidbody rb = obj.AddComponent<Rigidbody>();
         rb.interpolation = RigidbodyInterpolation.Extrapolate;
         rb.collisionDetectionMode = CollisionDetectionMode.Continuous;*/

        //�ݶ��̴� �ֱ�
        if (curType == TowerType.Turret)
        {
            BoxCollider boxCollider = obj.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(0.033f, 0.085f, 0.033f);
            boxCollider.center = new Vector3(0f, 0.0425f, 0f);
        }
        else if (curType == TowerType.WizzardTower)
        {
            CapsuleCollider capsuleCollider = obj.AddComponent<CapsuleCollider>();
            capsuleCollider.radius = 1.4f;
            capsuleCollider.height = 9f;
            capsuleCollider.center = new Vector3(0f, 4.45f, 0f);
        }


        // ��ƼŬ �ý��� ���� �� ����
        ParticleSystem particleSystem = particleSystemInstance;
        particleSystem.Stop();
        ParticleSystem particleSystem2 = particleSystemInstance2;
        particleSystem2.Stop();
        Destroy(particleSystemInstance.gameObject, particleSystem.main.startLifetime.constantMax); // ��� ��ƼŬ�� ����� �Ŀ� ��ƼŬ �ý��� ��ü ����
        Destroy(particleSystemInstance2.gameObject, particleSystem.main.startLifetime.constantMax); // ��� ��ƼŬ�� ����� �Ŀ� ��ƼŬ �ý��� ��ü ����

        //������Ʈ ������ ������ ��ư �ٽ� ����
        GameObject spawnBtn = FindObject(canvas, "SpawnTurretButton");
        if (spawnBtn != null)
        {
            spawnBtn.SetActive(true);
        }
    }

    private void DisableColliders()
    {
        SetCollidersEnable(false);
    }
    private void EnableColliders()
    {
        SetCollidersEnable(true);
    }
    //���ùڽ��� �ݶ��̴����� ���� ����
    private void SetCollidersEnable(bool v)
    {
        Collider[] childColliders = selectObj.GetComponentsInChildren<Collider>(true);
        Collider[] mainColliders = selectObj.GetComponents<Collider>();

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

    public void TowerSpawn()
    {
        // Canvas ������ SpawnTurretButton �̸��� ���� ������Ʈ ã��
        GameObject listPanel = FindObject(canvas, "TowerSpawnPanel");
        GameObject selectBtn = FindObject(canvas, "ConfirmSpawnButton");
        GameObject cancelBtn = FindObject(canvas, "CancelButton");

        if (!turretSpawn)
        {
            RaycastHit hit;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            if (!RaycastWithoutTriggers(ray, out hit)) return;

            selectObj = Instantiate(select, hit.collider.gameObject.transform.position + new Vector3(0, 0.005f, 0), select.transform.rotation);

            //�ͷ��϶� ���ùڽ� ����
            if (curType == TowerType.Turret)
            {
                //�״��
            }
            //���ڵ� Ÿ���϶� ���ùڽ� ����
            else if (curType == TowerType.WizzardTower)
            {
                //3�� Ű��
                Vector3 currentScale = selectObj.transform.localScale;
                selectObj.transform.localScale = new Vector3(currentScale.x * 3, currentScale.y, currentScale.z * 3);
            }

            DisableColliders();

            //���ùڽ� ����
            Renderer rend = selectObj.GetComponent<Renderer>();

            //�ͷ��϶� �������� üũ�ؼ� ���� ����
            if (curType == TowerType.Turret)
            {
                if (!hit.collider.gameObject.CompareTag("platform"))
                {
                    Color newColor = new Color(107f / 255f, 249f / 255f, 121f / 255f);
                    rend.material.color = newColor;
                }
                else
                {
                    Color newColor = new Color(238f / 255f, 74f / 255f, 88f / 255f);
                    rend.material.color = newColor;
                }
            }
            //���ڵ� Ÿ���϶� �������� üũ
            else if (curType == TowerType.WizzardTower)
            {
                if (CheckSurroundingBlocks("platform", hit, 0.045f))
                {
                    Color newColor = new Color(107f / 255f, 249f / 255f, 121f / 255f);
                    rend.material.color = newColor;
                }
                else
                {
                    Color newColor = new Color(238f / 255f, 74f / 255f, 88f / 255f);
                    rend.material.color = newColor;
                }
            }

            turretSpawn = true;

            //Ÿ�� ��ϲ��� ��ġ, ��� ��ư Ȱ��ȭ
            if (listPanel != null && selectBtn != null && cancelBtn != null)
            {
                listPanel.SetActive(false);
                selectBtn.SetActive(true);
                cancelBtn.SetActive(true);
            }
        }
        /*else
        {
            turretSpawn = false;
            Destroy(selectObj);
            selectObj = null;

            if (listPanel != null && selectBtn != null && cancelBtn != null)
            {
                listPanel.SetActive(true);
                selectBtn.SetActive(false);
                cancelBtn.SetActive(false);
            }
        }*/
    }
    public void ConfirmSelect()
    {
        // Canvas ��ưã��
        GameObject spawnBtn = FindObject(canvas, "SpawnTurretButton");
        GameObject selectBtn = FindObject(canvas, "ConfirmSpawnButton");
        GameObject cancelBtn = FindObject(canvas, "CancelButton");

        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (!RaycastWithoutTriggers(ray, out hit)) return;

        //ray�� ���� �κ��� platform�϶�
        if (hit.collider.gameObject.CompareTag("platform") && Vector3.Distance(hit.normal, Vector3.up) < 0.01f)
        {
            //�ͷ��϶�
            if (curType == TowerType.Turret)
            {
                focusObs = Instantiate(turret, Vector3.zero, Quaternion.Euler(0, -Camera.main.transform.eulerAngles.y, 0));
            }
            //���ڵ� Ÿ���϶�
            else if (curType == TowerType.WizzardTower)
            {
                //��¥��ġ ������ �κ��̸�
                if (CheckSurroundingBlocks("platform", hit, 0.045f))
                {
                    focusObs = Instantiate(wizzardTower, Vector3.zero, Quaternion.Euler(0, -Camera.main.transform.eulerAngles.y, 0));
                }
                //��ġ �Ұ����� �κ�
                else
                {
                    return;
                }
            }
            //���������� ��ġ �غ� �Ǹ�
            Destroy(selectObj);

            // ��ƼŬ �ý��� ���� �� ���
            ParticleSystem particleSystemInstance = Instantiate(particlePrefab, hit.collider.gameObject.transform.position + new Vector3(0, 0.001f, 0), particlePrefab.transform.rotation);
            particleSystemInstance.Play();
            ParticleSystem particleSystemInstance2 = Instantiate(particlePrefab2, hit.collider.gameObject.transform.position + new Vector3(0, 0.001f, 0), particlePrefab2.transform.rotation);
            particleSystemInstance2.Play();

            // �Ҹ� ���
            AudioSource audioSource = focusObs.GetComponent<AudioSource>();
            if (audioSource != null && spawnSound != null)
            {
                audioSource.PlayOneShot(spawnSound);
            }

            //�ͷ��϶�
            if (curType == TowerType.Turret)
            {
                hit.collider.gameObject.tag = "Occupied";

                Vector3 startPosition = hit.collider.gameObject.transform.position + new Vector3(0, -0.09f, 0); // ���� ��ġ ����
                focusObs.transform.position = startPosition;

                // ��ƼŬ �ý��� �ν��Ͻ��� �ڷ�ƾ���� ����
                StartCoroutine(MoveObjectToPosition(focusObs, hit.collider.gameObject.transform.position, 11, particleSystemInstance, particleSystemInstance2)); // 3�� ���� ��ǥ ��ġ�� �̵�
            }
            //���ڵ� Ÿ���϶�
            else if (curType == TowerType.WizzardTower)
            {
                Vector3 centerPosition = hit.collider.gameObject.transform.position;
                //hit�� �κп� �ִ� �÷����� �������� �� ����
                Collider[] hitColliders = Physics.OverlapSphere(centerPosition, 0.04f);
                //���� ��� ��� ������Ʈ�� üũ
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.gameObject.tag == "platform")
                    {
                        hitCollider.gameObject.tag = "Occupied";
                    }
                }

                Vector3 startPosition = hit.collider.gameObject.transform.position + new Vector3(0, -0.35f, 0); // ���� ��ġ ����
                focusObs.transform.position = startPosition;

                particleSystemInstance.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
                particleSystemInstance2.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);

                // ��ƼŬ �ý��� �ν��Ͻ��� �ڷ�ƾ���� ����
                StartCoroutine(MoveObjectToPosition(focusObs, hit.collider.gameObject.transform.position, 11, particleSystemInstance, particleSystemInstance2)); // 3�� ���� ��ǥ ��ġ�� �̵�
            }

            //���������� 
            if (spawnBtn != null && selectBtn != null && cancelBtn != null)
            {
                selectBtn.SetActive(false);
                cancelBtn.SetActive(false);
            }

            selectObj = null;
            focusObs = null;
            turretSpawn = false;
        }
        //ray�� ���� �κ��� platform�� �ƴϸ�
        else
        {
            return;
        }
    }

    // ��������� ������Ʈ�� ã�� �Լ�
    GameObject FindObject(GameObject parent, string name)
    {
        if (parent.name == name) return parent;
        foreach (Transform child in parent.transform)
        {
            GameObject found = FindObject(child.gameObject, name);
            if (found != null) return found;
        }
        return null;
    }

    //�������� üũ �Լ�
    bool CheckSurroundingBlocks(string requiredTag, RaycastHit hit, float checkRadius)
    {
        Vector3 centerPosition = hit.collider.gameObject.transform.position;
        Collider[] hitColliders = Physics.OverlapSphere(centerPosition, checkRadius);
        int sameTagCount = 0; // Ư�� �±׸� ���� ���� ��

        foreach (var hitCollider in hitColliders)
        {
            // �߾� ���� ������ �ֺ� ���� Ư�� �±׸� ������ �ִ��� Ȯ��
            if (hitCollider.gameObject != hit.collider.gameObject && hitCollider.tag == requiredTag)
            {
                sameTagCount++;
            }
        }

        // �ֺ� 8�� �� ��ΰ� Ư�� �±׸� �������� Ȯ��
        if (sameTagCount == 8)
        {
            Debug.Log(sameTagCount);
            return true;
        }
        else
        {
            Debug.Log(sameTagCount);
            return false;
        }
    }

    //��ư���� Ÿ������� ���
    public void GetTowerList()
    {
        if (!turretSpawn)
        {
            GameObject spawnBtn = FindObject(canvas, "SpawnTurretButton");
            GameObject listPanel = FindObject(canvas, "TowerSpawnPanel");

            if (spawnBtn != null && listPanel != null)
            {
                spawnBtn.SetActive(false);
                listPanel.SetActive(true);
            }
        }
        else { turretSpawn = false; }
    }
    //��ư����
    public void ResetButton()
    {
        if (turretSpawn)
        {
            //���� ��� ��ư ��������
            GameObject spawnBtn = FindObject(canvas, "SpawnTurretButton");
            GameObject listPanel = FindObject(canvas, "TowerSpawnPanel");
            GameObject spawnSelectBtn = FindObject(canvas, "ConfirmSpawnButton");
            GameObject spawncancelBtn = FindObject(canvas, "CancelButton");

            if (spawnBtn != null && listPanel != null && spawnSelectBtn != null && spawncancelBtn != null)
            {
                spawnBtn.SetActive(true);
                listPanel.SetActive(false);
                spawnSelectBtn.SetActive(false);
                spawncancelBtn.SetActive(false);
            }

            turretSpawn = false;
            focusObs = null;
            Destroy(focusObs);
            Destroy(selectObj);
            select = null;

        }
    }

    //������ Ÿ�� ����
    public void setTurret()
    {
        if (!turretSpawn)
        {
            curType = TowerType.Turret;
            TowerSpawn();
        }
    }

    public void setWizzardTower()
    {
        if (!turretSpawn)
        {
            curType = TowerType.WizzardTower;
            TowerSpawn();
        }
    }
}
