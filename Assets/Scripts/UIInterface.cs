using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInterface : MonoBehaviour
{
    public GameObject tower;
    public GameObject select;

    GameObject focusObs;

    public GameObject canvas;

    private bool turretSpawn = false;

    public ParticleSystem particlePrefab;
    public ParticleSystem particlePrefab2;

    public AudioClip spawnSound;

    void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
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
                // hit한 오브젝트의 바닥 부분을 확인하기 위한 레이캐스트
                RaycastHit floorHit;
                // 레이를 hit한 오브젝트의 바로 아래로 발사 (예를 들어, y축으로 -1)
                Ray downRay = new Ray(hitObject.transform.position, -Vector3.up);
                if (Physics.Raycast(downRay, out floorHit))
                {
                    Renderer rend = focusObs.GetComponent<Renderer>();
                    // 바닥 오브젝트가 'platform' 또는 'Occupied' 태그를 가지고 있는지 확인
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
                // hit한 오브젝트가 'platform' 또는 'Occupied'인 경우의 처리
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
                Vector3 startPosition = hit.point + new Vector3(0, -0.04f, 0); // 시작 위치 조정
                focusObs = Instantiate(tower, startPosition, tower.transform.rotation);
                hit.collider.gameObject.tag = "Occupied";

                // 파티클 시스템 생성 및 재생
                ParticleSystem particleSystemInstance = Instantiate(particlePrefab, hit.point + new Vector3(0, 0.001f, 0), particlePrefab.transform.rotation);
                particleSystemInstance.Play();
                ParticleSystem particleSystemInstance2 = Instantiate(particlePrefab2, hit.point + new Vector3(0, 0.001f, 0), particlePrefab2.transform.rotation);
                particleSystemInstance2.Play();

                // 소리 재생
                AudioSource audioSource = focusObs.GetComponent<AudioSource>();
                if (audioSource != null && spawnSound != null)
                {
                    audioSource.PlayOneShot(spawnSound);
                }

                // 파티클 시스템 인스턴스를 코루틴으로 전달
                StartCoroutine(MoveObjectToPosition(focusObs, hit.point, 11, particleSystemInstance, particleSystemInstance2)); // 3초 동안 목표 위치로 이동
            }
            else
            {
                Destroy(focusObs);
            }
            focusObs = null;
        }*/


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
                        Renderer rend = focusObs.GetComponent<Renderer>();
                        focusObs.transform.position = hit.collider.gameObject.transform.position + new Vector3(0, 0.005f, 0);

                        if (floorHit.collider.gameObject.transform.parent != null)
                        {
                            focusObs.transform.rotation = floorHit.collider.gameObject.transform.parent.rotation;
                        }

                        Color newColor = floorHit.collider.gameObject.CompareTag("platform") ?
                            new Color(238f / 255f, 74f / 255f, 88f / 255f) : // platform일 경우
                            new Color(107f / 255f, 249f / 255f, 121f / 255f); // Occupied일 경우
                        rend.material.color = newColor;
                        break; // 첫 번째로 찾은 오브젝트에 대해 처리를 완료했으므로 반복문 종료
                    }
                }
            }
            else
            {
                // hit한 오브젝트가 'platform' 또는 'Occupied'인 경우의 처리
                focusObs.transform.position = hit.collider.gameObject.transform.position + new Vector3(0, 0.005f, 0);

                if (hit.collider.gameObject.transform.parent != null)
                {
                    focusObs.transform.rotation = hit.collider.gameObject.transform.parent.rotation;
                }

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

        BoxCollider boxCollider = obj.AddComponent<BoxCollider>();
        /*boxCollider.size = new Vector3(0.08f, 0.1f, 0.08f);
        boxCollider.center = new Vector3(0f, 0.05f, 0f);*/
        boxCollider.size = new Vector3(0.033f, 0.085f, 0.033f);
        boxCollider.center = new Vector3(0f, 0.0425f, 0f);

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

    public void TurretSpawn()
    {
        // Canvas 내에서 SpawnTurretButton 이름을 가진 오브젝트 찾기
        GameObject spawnBtn = FindObject(canvas, "SpawnTurretButton");
        GameObject selectBtn = FindObject(canvas, "ConfirmSpawnButton");
        GameObject cancelBtn = FindObject(canvas, "CancelButton");

        if (!turretSpawn)
        {
            RaycastHit hit;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            if (!RaycastWithoutTriggers(ray, out hit)) return;
            focusObs = Instantiate(select, hit.collider.gameObject.transform.position + new Vector3(0, 0.005f, 0), select.transform.rotation);

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

            turretSpawn = true;

            if (spawnBtn != null && selectBtn != null && cancelBtn != null)
            {
                spawnBtn.SetActive(false);
                selectBtn.SetActive(true);
                cancelBtn.SetActive(true);
            }       
        }
        else
        {
            turretSpawn = false;
            Destroy(focusObs);
            focusObs = null;

            if (spawnBtn != null && selectBtn != null && cancelBtn != null)
            {
                spawnBtn.SetActive(true);
                selectBtn.SetActive(false);
                cancelBtn.SetActive(false);
            }
        }
    }
    public void ConfirmSelect()
    {
        Debug.Log("실행1");
        // Canvas 내에서 SpawnTurretButton 이름을 가진 오브젝트 찾기
        GameObject spawnBtn = FindObject(canvas, "SpawnTurretButton");
        GameObject selectBtn = FindObject(canvas, "ConfirmSpawnButton");
        GameObject cancelBtn = FindObject(canvas, "CancelButton");
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (!RaycastWithoutTriggers(ray, out hit)) return;
        
        if (hit.collider.gameObject.CompareTag("platform") && hit.normal.Equals(new Vector3(0, 1, 0)))
        {
            Debug.Log("실행2");
            Destroy(focusObs);
            Vector3 startPosition = hit.collider.gameObject.transform.position + new Vector3(0, -0.09f, 0); // 시작 위치 조정
            focusObs = Instantiate(tower, startPosition, Quaternion.Euler(0, -Camera.main.transform.eulerAngles.y, 0));
            hit.collider.gameObject.tag = "Occupied";

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

            // 파티클 시스템 인스턴스를 코루틴으로 전달
            StartCoroutine(MoveObjectToPosition(focusObs, hit.collider.gameObject.transform.position, 11, particleSystemInstance, particleSystemInstance2)); // 3초 동안 목표 위치로 이동

            if (spawnBtn != null && selectBtn != null && cancelBtn != null)
            {
                spawnBtn.SetActive(true);
                selectBtn.SetActive(false);
                cancelBtn.SetActive(false);
            }
        }
        else
        {
            Destroy(focusObs);
        }
        focusObs = null;
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
}
