using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInterface : MonoBehaviour
{
    public GameObject turret;
    public GameObject wizzardTower;
    public GameObject select;

    GameObject focusObs;
    GameObject selectObj;

    public GameObject canvas;

    private bool turretSpawn = false;

    public ParticleSystem particlePrefab;
    public ParticleSystem particlePrefab2;

    public AudioClip spawnSound;

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
            if (!hitObject.CompareTag("platform") && !hitObject.CompareTag("Occupied"))
            {
                // 레이를 hit한 오브젝트의 바로 아래로 발사 (예를 들어, y축으로 -1)
                Ray downRay = new Ray(hitObject.transform.position, -Vector3.up);
                RaycastHit[] hits = Physics.RaycastAll(downRay, Mathf.Infinity);
                foreach (var floorHit in hits)
                {
                    // 'platform' 또는 'Occupied' 태그를 가진 오브젝트가 있는지 검사
                    if (floorHit.collider.gameObject.CompareTag("platform") || floorHit.collider.gameObject.CompareTag("Occupied"))
                    {
                        Renderer rend = selectObj.GetComponent<Renderer>();
                        selectObj.transform.position = hit.collider.gameObject.transform.position + new Vector3(0, 0.005f, 0);

                        if (floorHit.collider.gameObject.transform.parent != null)
                        {
                            selectObj.transform.rotation = floorHit.collider.gameObject.transform.parent.rotation;
                        }

                        //터렛일때 스폰범위 체크
                        if (curType == TowerType.Turret)
                        {
                            Color newColor = floorHit.collider.gameObject.CompareTag("platform") ?
                            new Color(107f / 255f, 249f / 255f, 121f / 255f) : // platform일 경우
                            new Color(238f / 255f, 74f / 255f, 88f / 255f);  // Occupied일 경우
                            rend.material.color = newColor;
                            break; // 첫 번째로 찾은 오브젝트에 대해 처리를 완료했으므로 반복문 종료
                        }
                        //위자드 타워일때 스폰범위 체크
                        else if (curType == TowerType.WizzardTower)
                        {
                            //platform일때
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
                            //Ocuppied일때
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
                // hit한 오브젝트가 'platform' 또는 'Occupied'인 경우의 처리
                selectObj.transform.position = hit.collider.gameObject.transform.position + new Vector3(0, 0.005f, 0);

                if (hit.collider.gameObject.transform.parent != null)
                {
                    selectObj.transform.rotation = hit.collider.gameObject.transform.parent.rotation;
                }

                Renderer rend = selectObj.GetComponent<Renderer>();

                //터렛일때 스폰범위 체크
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
                //위자드 타워일때 스폰범위 체크
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

        obj.transform.position = targetPosition; // 최종 위치 보정

        // 모든 이동이 완료된 후 Rigidbody와 BoxCollider 추가
       /* Rigidbody rb = obj.AddComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Extrapolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;*/

        if(curType == TowerType.Turret)
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
        

        // 파티클 시스템 정지 및 제거
        ParticleSystem particleSystem = particleSystemInstance;
        particleSystem.Stop();
        ParticleSystem particleSystem2 = particleSystemInstance2;
        particleSystem2.Stop();
        Destroy(particleSystemInstance.gameObject, particleSystem.main.startLifetime.constantMax); // 모든 파티클이 사라진 후에 파티클 시스템 객체 제거
        Destroy(particleSystemInstance2.gameObject, particleSystem.main.startLifetime.constantMax);
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
        // Canvas 내에서 SpawnTurretButton 이름을 가진 오브젝트 찾기
        GameObject listPanel = FindObject(canvas, "TowerSpawnPanel");
        GameObject selectBtn = FindObject(canvas, "ConfirmSpawnButton");
        GameObject cancelBtn = FindObject(canvas, "CancelButton");

        if (!turretSpawn)
        {
            RaycastHit hit;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            if (!RaycastWithoutTriggers(ray, out hit)) return;

            //터렛일때 선택박스 설정
            if (curType == TowerType.Turret)
            {
                selectObj = Instantiate(select, hit.collider.gameObject.transform.position + new Vector3(0, 0.005f, 0), select.transform.rotation);
            }
            //위자드 타워일때 선택박스 설정
            else if (curType == TowerType.WizzardTower)
            {
                selectObj = Instantiate(select, hit.collider.gameObject.transform.position + new Vector3(0, 0.005f, 0), select.transform.rotation);

                Vector3 currentScale = selectObj.transform.localScale;
                selectObj.transform.localScale = new Vector3(currentScale.x * 3, currentScale.y, currentScale.z * 3);
            }

            DisableColliders();

            Renderer rend = selectObj.GetComponent<Renderer>();

            //터렛일때 스폰범위 체크
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
            //위자드 타워일때 스폰범위 체크
            else if (curType == TowerType.WizzardTower)
            {
                if(CheckSurroundingBlocks("platform", hit, 0.045f))
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

            if (listPanel != null && selectBtn != null && cancelBtn != null)
            {
                listPanel.SetActive(false);
                selectBtn.SetActive(true);
                cancelBtn.SetActive(true);
            }       
        }
        else
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
        }
    }
    public void ConfirmSelect()
    {
        // Canvas 내에서 SpawnTurretButton 이름을 가진 오브젝트 찾기
        GameObject spawnBtn = FindObject(canvas, "SpawnTurretButton");
        GameObject selectBtn = FindObject(canvas, "ConfirmSpawnButton");
        GameObject cancelBtn = FindObject(canvas, "CancelButton");
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (!RaycastWithoutTriggers(ray, out hit)) return;
        if (hit.collider.gameObject.CompareTag("platform") && Vector3.Distance(hit.normal, Vector3.up) < 0.01f)
        {
            Destroy(selectObj);

            //터렛일때
            if (curType == TowerType.Turret)
            {
                focusObs = Instantiate(turret, Vector3.zero, Quaternion.Euler(0, -Camera.main.transform.eulerAngles.y, 0));
            }
            //위자드 타워일때
            else if (curType == TowerType.WizzardTower)
            {
                focusObs = Instantiate(wizzardTower, Vector3.zero, Quaternion.Euler(0, -Camera.main.transform.eulerAngles.y, 0));
            }

            // 파티클 시스템 생성 및 재생
            ParticleSystem particleSystemInstance = Instantiate(particlePrefab, hit.collider.gameObject.transform.position + new Vector3(0, 0.001f, 0), particlePrefab.transform.rotation);
            particleSystemInstance.Play();
            ParticleSystem particleSystemInstance2 = Instantiate(particlePrefab2, hit.collider.gameObject.transform.position + new Vector3(0, 0.001f, 0), particlePrefab2.transform.rotation);
            particleSystemInstance2.Play();

            // 소리 재생
            AudioSource audioSource = focusObs.GetComponent<AudioSource>();
            if (audioSource != null && spawnSound != null)
            {
                audioSource.PlayOneShot(spawnSound);
            }

            //터렛일때
            if (curType == TowerType.Turret)
            {
                hit.collider.gameObject.tag = "Occupied";

                Vector3 startPosition = hit.collider.gameObject.transform.position + new Vector3(0, -0.09f, 0); // 시작 위치 조정
                focusObs.transform.position = startPosition;

                // 파티클 시스템 인스턴스를 코루틴으로 전달
                StartCoroutine(MoveObjectToPosition(focusObs, hit.collider.gameObject.transform.position, 11, particleSystemInstance, particleSystemInstance2)); // 3초 동안 목표 위치로 이동
            }
            //위자드 타워일때
            else if (curType == TowerType.WizzardTower)
            {
                Vector3 centerPosition = hit.collider.gameObject.transform.position;
                Collider[] hitColliders = Physics.OverlapSphere(centerPosition, 0.04f);

                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.gameObject.tag == "platform")
                    {
                        hitCollider.gameObject.tag = "Occupied";
                    }
                }

                Vector3 startPosition = hit.collider.gameObject.transform.position + new Vector3(0, -0.35f, 0); // 시작 위치 조정
                focusObs.transform.position = startPosition;

                particleSystemInstance.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
                particleSystemInstance2.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);

                // 파티클 시스템 인스턴스를 코루틴으로 전달
                StartCoroutine(MoveObjectToPosition(focusObs, hit.collider.gameObject.transform.position, 11, particleSystemInstance, particleSystemInstance2)); // 3초 동안 목표 위치로 이동
            }

            if (spawnBtn != null && selectBtn != null && cancelBtn != null)
            {
                spawnBtn.SetActive(true);
                selectBtn.SetActive(false);
                cancelBtn.SetActive(false);
            }
        }
        else if (hit.collider.gameObject.CompareTag("platform")){
        }
        else
        {
            Destroy(selectObj);
        }
        selectObj = null;
        turretSpawn = false;
    }

    // 재귀적으로 오브젝트를 찾는 함수
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

    //스폰범위 체크 함수
    bool CheckSurroundingBlocks(string requiredTag, RaycastHit hit, float checkRadius)
    {
        Vector3 centerPosition = hit.collider.gameObject.transform.position;
        Collider[] hitColliders = Physics.OverlapSphere(centerPosition, checkRadius);
        int sameTagCount = 0; // 특정 태그를 가진 블럭의 수

        foreach (var hitCollider in hitColliders)
        {
            // 중앙 블럭을 제외한 주변 블럭이 특정 태그를 가지고 있는지 확인
            if (hitCollider.gameObject != hit.collider.gameObject && hitCollider.tag == requiredTag)
            {
                sameTagCount++;
            }
        }

        // 주변 8개 블럭 모두가 특정 태그를 가졌는지 확인
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

    //버튼관련
    public void GetTowerList()
    {
        GameObject spawnBtn = FindObject(canvas, "SpawnTurretButton");
        GameObject listPanel = FindObject(canvas, "TowerSpawnPanel");

        if (spawnBtn != null && listPanel != null)
        {
            spawnBtn.SetActive(false);
            listPanel.SetActive(true);
        }
    }
    //버튼관련
    public void ResetButton()
    {
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
    }

    //스폰할 타워 설정
    public void setTurret()
    {
        curType = TowerType.Turret;
        TowerSpawn();
    }

    public void setWizzardTower()
    {
        curType = TowerType.WizzardTower;
        TowerSpawn();
    }
}
