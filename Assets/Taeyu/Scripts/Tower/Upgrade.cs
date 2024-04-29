using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    public GameObject turretV1;
    public GameObject turretV2;
    public GameObject turretV3;
    public GameObject turretV4;

    public ParticleSystem select;
    public ParticleSystem catchTurretEffect;

    GameObject focusObs;
    ParticleSystem selectPaticle;
    ParticleSystem catchPaticle;

    public GameObject canvas;

    private bool turretUpgrade = false;

    public ParticleSystem particlePrefab;

    public AudioClip spawnSound;

    void Update()
    {
        if (turretUpgrade)
        {
            
            RaycastHit hit;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (!RaycastWithoutTriggers(ray, out hit)) return;

            GameObject hitObject = hit.collider.gameObject;

            if (!hitObject.CompareTag("Turret"))
            {
                // 레이를 hit한 오브젝트의 바로 아래로 발사 (예를 들어, y축으로 -1)
                Ray downRay = new Ray(hitObject.transform.position, -Vector3.up);
                RaycastHit[] hits = Physics.RaycastAll(downRay, Mathf.Infinity);
                if (hits.Length == 0)
                {
                    GameObject closeTurret;
                    if (findSurrounding("Turret", hit, out closeTurret))
                    {
                        selectPaticle.transform.position = closeTurret.transform.position + new Vector3(0, 0.005f, 0);
                        catchPaticle.transform.position = closeTurret.transform.position + new Vector3(0, 0.005f, 0);
                        catchPaticle.Play();
                    }
                    else
                    {
                        selectPaticle.transform.position = hit.point + new Vector3(0, 0.005f, 0);
                        catchPaticle.Stop();
                    }
                }
                else {
                    foreach (var floorHit in hits)
                    {
                        //Turret이면
                        if (floorHit.collider.gameObject.CompareTag("Turret"))
                        {
                            selectPaticle.transform.position = hit.collider.gameObject.transform.position + new Vector3(0, 0.005f, 0);
                            catchPaticle.transform.position = hit.point + new Vector3(0, 0.005f, 0);
                            catchPaticle.Play();

                            break;
                        }
                        else
                        {
                            GameObject closeTurret;
                            if (findSurrounding("Turret", hit, out closeTurret))
                            {
                                selectPaticle.transform.position = closeTurret.transform.position + new Vector3(0, 0.005f, 0);
                                catchPaticle.transform.position = closeTurret.transform.position + new Vector3(0, 0.005f, 0);
                                catchPaticle.Play();
                            }
                            else
                            {
                                selectPaticle.transform.position = hit.point + new Vector3(0, 0.005f, 0);
                                catchPaticle.Stop();
                            }
                        }
                    }
                }
            }
            else
            {
                selectPaticle.transform.position = hit.collider.gameObject.transform.position + new Vector3(0, 0.005f, 0);
                catchPaticle.transform.position = hit.point + new Vector3(0, 0.005f, 0);
                catchPaticle.Play();
            }
        }
    }

    IEnumerator MoveObjectToPosition(GameObject obj, float duration, ParticleSystem particleSystemInstance)
    {
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            yield return null;
        }

        BoxCollider boxCollider = obj.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(0.033f, 0.085f, 0.033f);
        boxCollider.center = new Vector3(0f, 0.0425f, 0f);

        // 파티클 시스템 정지 및 제거
        ParticleSystem particleSystem = particleSystemInstance;
        particleSystem.Stop();
        Destroy(particleSystemInstance.gameObject, particleSystem.main.startLifetime.constantMax); // 모든 파티클이 사라진 후에 파티클 시스템 객체 제거
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

    public void TurretUpgrade()
    {
        // Canvas 내에서 SpawnTurretButton 이름을 가진 오브젝트 찾기
        GameObject upgradeBtn = FindObject(canvas, "UpgradeTurretButton");
        GameObject selectBtn = FindObject(canvas, "UpgradeConfirmButton");
        GameObject cancelBtn = FindObject(canvas, "UpgradeCancelButton");

        if (!turretUpgrade)
        {
            RaycastHit hit;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            if (!RaycastWithoutTriggers(ray, out hit)) return;
            selectPaticle = Instantiate(select, hit.point + new Vector3(0, 0.005f, 0), select.transform.rotation);
            selectPaticle.Play();
            catchPaticle = Instantiate(catchTurretEffect, hit.point + new Vector3(0, 0.005f, 0), select.transform.rotation); 

            turretUpgrade = true;

            if (upgradeBtn != null && selectBtn != null && cancelBtn != null)
            {
                upgradeBtn.SetActive(false);
                selectBtn.SetActive(true);
                cancelBtn.SetActive(true);
            }
        }
        else
        {
            turretUpgrade = false;
            Destroy(selectPaticle);
            selectPaticle = null;
            if (catchPaticle != null)
            {
                Destroy(catchPaticle);
                catchPaticle = null;
            }

            if (upgradeBtn != null && selectBtn != null && cancelBtn != null)
            {
                upgradeBtn.SetActive(true);
                selectBtn.SetActive(false);
                cancelBtn.SetActive(false);
            }
        }
    }
    public void ConfirmSelect()
    {

        // Canvas 내에서 SpawnTurretButton 이름을 가진 오브젝트 찾기
        GameObject upgradeBtn = FindObject(canvas, "UpgradeTurretButton");
        GameObject selectBtn = FindObject(canvas, "ConfirmSpawnButton");
        GameObject cancelBtn = FindObject(canvas, "CancelButton");
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (!RaycastWithoutTriggers(ray, out hit)) return;

        GameObject closeTurret;
        if (findSurrounding("Turret", hit, out closeTurret))
        {
            Destroy(selectPaticle);
            Destroy(catchPaticle);

            if (closeTurret.name == "Turret_v1(Clone)")
            {
                focusObs = Instantiate(turretV2, closeTurret.transform.position, closeTurret.transform.rotation);
                Destroy(closeTurret);
                isUpgrade();
            }
            else if (closeTurret.name == "Turret_v2(Clone)")
            {
                focusObs = Instantiate(turretV3, closeTurret.transform.position, closeTurret.transform.rotation);
                Destroy(closeTurret);
                isUpgrade();
            }
            else if (closeTurret.name == "Turret_v3(Clone)")
            {
                focusObs = Instantiate(turretV4, closeTurret.transform.position, closeTurret.transform.rotation);
                Destroy(closeTurret);
                isUpgrade();
            }
            else
            {
                Debug.Log("최고레벨 도달");
            }
            void isUpgrade()
            {
                // 파티클 시스템 생성 및 재생
                ParticleSystem particleSystemInstance = Instantiate(particlePrefab, closeTurret.transform.position + new Vector3(0, 0.001f, 0), particlePrefab.transform.rotation);
                particleSystemInstance.Play();
                // 소리 재생
                AudioSource audioSource = focusObs.GetComponent<AudioSource>();
                if (audioSource != null && spawnSound != null)
                {
                    audioSource.PlayOneShot(spawnSound);
                }
                // 파티클 시스템 인스턴스를 코루틴으로 전달
                StartCoroutine(MoveObjectToPosition(focusObs, 2, particleSystemInstance)); // 3초 동안 목표 위치로 이동
            }
            if (upgradeBtn != null && selectBtn != null && cancelBtn != null)
            {
                upgradeBtn.SetActive(true);
                selectBtn.SetActive(false);
                cancelBtn.SetActive(false);
            }
            else
            {
                Destroy(selectPaticle);
            }
            selectPaticle = null;
            turretUpgrade = false;
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

    bool findSurrounding(string requiredTag, RaycastHit hit, out GameObject closestTurret)
    {
        Vector3 centerPosition = hit.point;
        Collider[] hitColliders = Physics.OverlapSphere(centerPosition, 0.04f);
        float closestDistance = Mathf.Infinity; // 초기 거리는 무한대로 설정
        GameObject closest = null; // 가장 가까운 객체를 저장할 변수

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.tag == requiredTag)
            {
                float distance = Vector3.Distance(centerPosition, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance; // 가장 가까운 거리 업데이트
                    closest = hitCollider.gameObject; // 가장 가까운 객체 업데이트
                }
            }
        }

        closestTurret = closest; // 가장 가까운 터렛을 out 파라미터로 설정
        return closest != null; // 찾은 경우 true, 아니면 false 반환
    }
}
