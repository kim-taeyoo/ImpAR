using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using static Unity.VisualScripting.Member;

public class UIInterface : MonoBehaviour
{
    //프리팹들
    public GameObject turret;
    public GameObject wizzardTower;
    public GameObject select;

    //설치될 오브젝트랑, 범위표시 오브젝트
    GameObject focusObs;
    GameObject selectObj;

    //버튼 있는 canvas 가져오기
    public GameObject canvas;

    private bool turretSpawn = false;

    //설치될때 이펙트
    public ParticleSystem particlePrefab;
    public ParticleSystem particlePrefab2;

    public AudioClip spawnSound;


    //타워 종류
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
            //ray가 닿은 부분이 플랫폼이나 이미 설치된 플랫폼이 아닐때(다른 오브젝트일때)
            if (!hitObject.CompareTag("platform") && !hitObject.CompareTag("Occupied"))
            {
                // 레이를 hit한 오브젝트의 바로 아래로 발사
                Ray downRay = new Ray(hitObject.transform.position, -Vector3.up);
                RaycastHit[] hits = Physics.RaycastAll(downRay, Mathf.Infinity);
                //아래로 쏜 ray가 닿은 모든 오브젝트를 검사
                foreach (var floorHit in hits)
                {
                    // 'platform' 또는 'Occupied' 태그를 가진 오브젝트가 있을때
                    if (floorHit.collider.gameObject.CompareTag("platform") || floorHit.collider.gameObject.CompareTag("Occupied"))
                    {
                        Renderer rend = selectObj.GetComponent<Renderer>();
                        // 설치범위 오브젝트를 해당 오브젝트의 위치로 이동
                        selectObj.transform.position = hit.collider.gameObject.transform.position + new Vector3(0, 0.005f, 0);

                        //플랫폼 오브젝트의 부모가 plane 이니깐 해당 오브젝트의 회전을 적용
                        if (floorHit.collider.gameObject.transform.parent != null)
                        {
                            selectObj.transform.rotation = floorHit.collider.gameObject.transform.parent.rotation;
                        }

                        //터렛일때 스폰범위 체크(색상변경)
                        if (curType == TowerType.Turret)
                        {
                            Color newColor = floorHit.collider.gameObject.CompareTag("platform") ?
                            new Color(107f / 255f, 249f / 255f, 121f / 255f) : // platform일 경우
                            new Color(238f / 255f, 74f / 255f, 88f / 255f);  // Occupied일 경우
                            rend.material.color = newColor;
                            break;
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

    IEnumerator MoveObjectToPosition(GameObject obj, Vector3 targetPosition, float duration, ParticleSystem particleSystemInstance, ParticleSystem particleSystemInstance2, AudioSource audioSource)
    {
        float delayTime = 0;
        while (delayTime < 1)
        {
            delayTime += Time.deltaTime;
            yield return null;
        }

            float time = 0;
        Vector3 startPosition = obj.transform.position;

        while (time < duration)
        {
            obj.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        obj.transform.position = targetPosition; // 최종 위치 보정

         /*rb.interpolation = RigidbodyInterpolation.Extrapolate;
         rb.collisionDetectionMode = CollisionDetectionMode.Continuous;*/

        //콜라이더 넣기
        if (curType == TowerType.Turret)
        {
            // 모든 이동이 완료된 후 Rigidbody와 BoxCollider 추가
            Rigidbody rb = obj.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;

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
        Destroy(particleSystemInstance2.gameObject, particleSystem.main.startLifetime.constantMax); // 모든 파티클이 사라진 후에 파티클 시스템 객체 제거


        //오브젝트 생성이 끝나면 버튼 다시 생김
        GameObject spawnBtn = FindObject(canvas, "SpawnTurretButton");
        if (spawnBtn != null)
        {
            spawnBtn.SetActive(true);
        }

        //태그주기
        if (curType == TowerType.Turret)
        {
            obj.tag = "Turret";
        }

        float currentTime = 0; // 현재 시간 카운터
        float startVolume = audioSource.volume; // 시작 볼륨 설정

        while (currentTime < 2)
        {
            currentTime += Time.deltaTime; // 시간 업데이트
            audioSource.volume = Mathf.Lerp(startVolume, 0, currentTime / 2);
            yield return null; // 다음 프레임까지 대기
        }

        audioSource.Stop(); // 볼륨이 최소가 되면 오디오 정지
        audioSource.volume = startVolume; // 원래 볼륨으로 복원
    }

    private void DisableColliders()
    {
        SetCollidersEnable(false);
    }
    private void EnableColliders()
    {
        SetCollidersEnable(true);
    }
    //선택박스의 콜라이더들을 빼는 역할
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

            selectObj = Instantiate(select, hit.collider.gameObject.transform.position + new Vector3(0, 0.005f, 0), select.transform.rotation);

            //터렛일때 선택박스 설정
            if (curType == TowerType.Turret)
            {
                //그대로
            }
            //위자드 타워일때 선택박스 설정
            else if (curType == TowerType.WizzardTower)
            {
                //3배 키움
                Vector3 currentScale = selectObj.transform.localScale;
                selectObj.transform.localScale = new Vector3(currentScale.x * 3, currentScale.y, currentScale.z * 3);
            }

            DisableColliders();

            //선택박스 색상
            Renderer rend = selectObj.GetComponent<Renderer>();

            //터렛일때 스폰범위 체크해서 색상 변경
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

            //타워 목록끄고 설치, 취소 버튼 활성화
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
        // Canvas 버튼찾기
        GameObject listPanel = FindObject(canvas, "TowerSpawnPanel");
        GameObject selectBtn = FindObject(canvas, "ConfirmSpawnButton");
        GameObject cancelBtn = FindObject(canvas, "CancelButton");

        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (!RaycastWithoutTriggers(ray, out hit)) return;

        //ray가 닿은 부분이 platform일때
        if (hit.collider.gameObject.CompareTag("platform") && Vector3.Distance(hit.normal, Vector3.up) < 0.01f)
        {
            //터렛일때 돈설정
            if (curType == TowerType.Turret)
            {
                if (GameManager.gm.money < 2000)
                {
                    return;
                }
                int spendMoney = -2000;
                UIManager.um.ChangeMoneyNum(GameManager.gm.money, spendMoney);
                GameManager.gm.money += spendMoney;
            }
            //위자드 타워일때 돈설정
            else if (curType == TowerType.WizzardTower)
            {
                if(GameManager.gm.money < 10000)
                {
                    return;
                }
                int spendMoney = -10000;
                UIManager.um.ChangeMoneyNum(GameManager.gm.money, spendMoney);
                GameManager.gm.money += spendMoney;
            }

            //터렛일때
            if (curType == TowerType.Turret)
            {
                focusObs = Instantiate(turret, Vector3.zero, Quaternion.Euler(0, -Camera.main.transform.eulerAngles.y, 0));
            }
            //위자드 타워일때
            else if (curType == TowerType.WizzardTower)
            {
                //진짜설치 가능한 부분이면
                if (CheckSurroundingBlocks("platform", hit, 0.045f))
                {
                    focusObs = Instantiate(wizzardTower, Vector3.zero, Quaternion.Euler(0, -Camera.main.transform.eulerAngles.y, 0));
                }
                //설치 불가능한 부분
                else
                {
                    return;
                }     
            }
            //정상적으로 설치 준비가 되면
            Destroy(selectObj);

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
                StartCoroutine(MoveObjectToPosition(focusObs, hit.collider.gameObject.transform.position, 4, particleSystemInstance, particleSystemInstance2, audioSource)); // 3초 동안 목표 위치로 이동
            }
            //위자드 타워일때
            else if (curType == TowerType.WizzardTower)
            {
                Vector3 centerPosition = hit.collider.gameObject.transform.position;
                //hit한 부분에 있는 플랫폼을 중점으로 구 생성
                Collider[] hitColliders = Physics.OverlapSphere(centerPosition, 0.04f);
                //구에 닿는 모든 오브젝트를 체크
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
                StartCoroutine(MoveObjectToPosition(focusObs, hit.collider.gameObject.transform.position, 4, particleSystemInstance, particleSystemInstance2, audioSource)); // 3초 동안 목표 위치로 이동
            }

            //정상적으로 
            if (listPanel != null && selectBtn != null && cancelBtn != null)
            {
                listPanel.SetActive(true);
                selectBtn.SetActive(false);
                cancelBtn.SetActive(false);
            }

            selectObj = null;
            focusObs = null;
            turretSpawn = false;
        }
        //ray가 닿은 부분이 platform이 아니면
        else
        {
            return;
        }
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
            return true;
        }
        else
        {
            return false;
        }
    }

    //버튼관련 타워목록을 띄움
   /* public void GetTowerList()
    {
        if (!turretSpawn)
        {
            GameObject spawnBtn = FindObject(canvas, "SpawnTurretButton");
            GameObject listPanel = FindObject(canvas, "TowerSpawnPanel");

            if (spawnBtn != null && listPanel != null)
            {
                //spawnBtn.SetActive(false); 일단 주석처리함 (이규빈)
                listPanel.SetActive(true);
            }
        }
        else { turretSpawn = false; }
    }*/
    //버튼관련
    public void ResetButton()
    {
        if (turretSpawn)
        {
            //관련 모든 버튼 가져오기
            GameObject listPanel = FindObject(canvas, "TowerSpawnPanel");
            GameObject spawnSelectBtn = FindObject(canvas, "ConfirmSpawnButton");
            GameObject spawncancelBtn = FindObject(canvas, "CancelButton");

            if (listPanel != null && spawnSelectBtn != null && spawncancelBtn != null)
            {
                listPanel.SetActive(true);
                spawnSelectBtn.SetActive(false);
                spawncancelBtn.SetActive(false);
            }

            turretSpawn = false;
            focusObs = null;
            Destroy(focusObs);
            Destroy(selectObj);
            selectObj = null;

        }
    }

    //스폰할 타워 설정
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
