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
                    selectPaticle.transform.position = hit.point + new Vector3(0, 0.005f, 0);
                    catchPaticle.Stop();
                    /*Renderer rend = selectPaticle.GetComponent<Renderer>();
                    Color newColor = new Color(107f / 255f, 249f / 255f, 121f / 255f);
                    rend.material.color = newColor;*/
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
                            /* if (floorHit.collider.gameObject.transform.parent != null)
                             {
                                 selectPaticle.transform.rotation = floorHit.collider.gameObject.transform.parent.rotation;
                             }*/

                            /* Renderer rend = selectPaticle.GetComponent<Renderer>();
                             Color newColor = new Color(238f / 255f, 74f / 255f, 88f / 255f);

                             rend.material.color = newColor;*/
                            break;
                        }
                        else
                        {
                            selectPaticle.transform.position = hit.point + new Vector3(0, 0.005f, 0);
                            catchPaticle.Stop();

                            /* Renderer rend = selectPaticle.GetComponent<Renderer>();
                             Color newColor = new Color(107f / 255f, 249f / 255f, 121f / 255f);
                             rend.material.color = newColor;*/

                            /* if (hit.collider.gameObject.transform.parent != null)
                        {
                            focusObs.transform.rotation = hit.collider.gameObject.transform.parent.rotation;
                        }*/
                        }
                    }
                }
            }
            else
            {
                selectPaticle.transform.position = hit.collider.gameObject.transform.position + new Vector3(0, 0.005f, 0);
                catchPaticle.transform.position = hit.point + new Vector3(0, 0.005f, 0);
                catchPaticle.Play();
                /* if (hit.collider.gameObject.transform.parent != null)
                 {
                     focusObs.transform.rotation = hit.collider.gameObject.transform.parent.rotation;
                 }*/

                /*Renderer rend = selectPaticle.GetComponent<Renderer>();
                if (!hit.collider.gameObject.CompareTag("Turret"))
                {
                    Color newColor = new Color(238f / 255f, 74f / 255f, 88f / 255f);
                    rend.material.color = newColor;
                }
                else
                {
                    Color newColor = new Color(107f / 255f, 249f / 255f, 121f / 255f);
                    rend.material.color = newColor;
                }*/
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

   /* private void DisableColliders()
    {
        SetCollidersEnable(false);
    }
    private void EnableColliders()
    {
        SetCollidersEnable(true);
    }

    private void SetCollidersEnable(bool v)
    {
        Collider[] childColliders = selectPaticle.GetComponentsInChildren<Collider>(true);
        Collider[] mainColliders = selectPaticle.GetComponents<Collider>();

        foreach (Collider collider in childColliders)
        {
            collider.enabled = v;
        }
        foreach (Collider collider in mainColliders)
        {
            collider.enabled = v;
        }
    }*/

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

            /*DisableColliders();*/

            /*Renderer rend = selectPaticle.GetComponent<Renderer>();
            if (!hit.collider.gameObject.CompareTag("platform"))
            {
                Color newColor = new Color(238f / 255f, 74f / 255f, 88f / 255f);
                rend.material.color = newColor;
            }
            else
            {
                Color newColor = new Color(107f / 255f, 249f / 255f, 121f / 255f);
                rend.material.color = newColor;
            }*/

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

        if (hit.collider.gameObject.CompareTag("Turret"))
        {
            Debug.Log(hit.collider.gameObject.name);

            Destroy(selectPaticle);
            Destroy(catchPaticle);

            if (hit.collider.gameObject.name == "Turret_v1(Clone)")
            {
                focusObs = Instantiate(turretV2, hit.collider.gameObject.transform.position, hit.collider.gameObject.transform.rotation);
                Destroy(hit.collider.gameObject);
                isUpgrade();
            }
            else if (hit.collider.gameObject.name == "Turret_v2(Clone)")
            {
                focusObs = Instantiate(turretV3, hit.collider.gameObject.transform.position, hit.collider.gameObject.transform.rotation);
                Destroy(hit.collider.gameObject);
                isUpgrade();
            }
            else if (hit.collider.gameObject.name == "Turret_v3(Clone)")
            {
                focusObs = Instantiate(turretV4, hit.collider.gameObject.transform.position, hit.collider.gameObject.transform.rotation);
                Destroy(hit.collider.gameObject);
                isUpgrade();
            }
            else
            {
                Debug.Log("최고레벨 도달");
            }
            void isUpgrade()
            {
                // 파티클 시스템 생성 및 재생
                ParticleSystem particleSystemInstance = Instantiate(particlePrefab, hit.collider.gameObject.transform.position + new Vector3(0, 0.001f, 0), particlePrefab.transform.rotation);
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
}
