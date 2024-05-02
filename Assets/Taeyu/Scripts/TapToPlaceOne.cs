using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;

using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class TapToPlaceOne : MonoBehaviour
{
    public GameObject canvas;

    private ARRaycastManager raycastManager;

    //선택영역표시 && 생성될 바닥
    [SerializeField] private GameObject prefabToInstantiate;
    [SerializeField] private GameObject prefabToPlaneInstantiate;


    private GameObject spawnedArea;

    // stores the results of ARRaycast
    private List<ARRaycastHit> hitResults = new List<ARRaycastHit>();

    // ARPlaneManager 참조 추가
    private ARPlaneManager planeManager;

    private bool isSpawnMap = false;
    private int map = 0;

    private float yRotation;

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    void Update()
    {
        if (isSpawnMap)
        {
            createAndUpdate();
        }
    }

    void createAndUpdate()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (raycastManager.Raycast(ray, hitResults, TrackableType.PlaneWithinPolygon) && hitResults.Count > 0)
        {
            // 오브젝트를 처음 생성할 때
            if (spawnedArea != null)
            {
                //Debug.Log("테스트");
                spawnedArea.transform.position = hitResults[0].pose.position + new Vector3(0, 0.001f, 0);
                // 카메라의 y축 회전 값을 가져옴
                yRotation = Camera.main.transform.eulerAngles.y;

                // 오브젝트의 회전을 카메라의 y축 회전 값으로 설정
                spawnedArea.transform.rotation = Quaternion.Euler(0, yRotation, 0);
            }
        }
    }


    public void isSelect()
    {
        if (isSpawnMap)
        {

            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            if (raycastManager.Raycast(ray, hitResults, TrackableType.PlaneWithinPolygon) && hitResults.Count > 0)
            {
                Destroy(spawnedArea);

                spawnedArea = Instantiate(prefabToPlaneInstantiate, hitResults[0].pose.position + new Vector3(0, 0.001f, 0), Quaternion.identity);
                // CreatePlane 스크립트에 접근하여 콜백 설정
                CreatePlane createPlaneScript = spawnedArea.GetComponent<CreatePlane>();
                if (createPlaneScript != null)
                {
                    createPlaneScript.onCreationComplete = () =>
                    {
                        // 큐브 생성이 완료된 후 회전 적용
                        spawnedArea.transform.rotation = Quaternion.Euler(0, yRotation, 0);
                    };
                }

                isSpawnMap = false;

                // ARPlaneManager 컴포넌트를 비활성화합니다.
                if (planeManager != null)
                {
                    planeManager.enabled = false;
                    foreach (var plane in planeManager.trackables)
                    {
                        Debug.Log(plane);
                        plane.gameObject.SetActive(false);
                    }
                }

                //버튼 관련
                GameObject makeMapBtn = FindObject(canvas, "MapButton");
                GameObject selectBtn = FindObject(canvas, "MapSelectButton");
                GameObject TowerListPanel = FindObject(canvas, "TowerSpawnPanel");
                GameObject upgradeBtn = FindObject(canvas, "UpgradeTurretButton");
                GameObject recoveryBtn = FindObject(canvas, "CastleHPRecovery");


                if (makeMapBtn != null && selectBtn != null && TowerListPanel != null && upgradeBtn != null && recoveryBtn != null)
                {
                    makeMapBtn.SetActive(false);
                    selectBtn.SetActive(false);
                    TowerListPanel.SetActive(true);
                    upgradeBtn.SetActive(true);
                    recoveryBtn.SetActive(true);
                }
                else
                {
                    Debug.Log("버튼을 찾을 수 없음");
                }
            }
            else { Debug.Log("정확한 위치에서 눌러주세요");  }
        }
    }

    public void isMap()
    {
        if (!isSpawnMap && map < 1)
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            // 카메라의 y축 회전 값을 가져옴
            yRotation = Camera.main.transform.eulerAngles.y;

            if (raycastManager.Raycast(ray, hitResults, TrackableType.PlaneWithinPolygon) && hitResults.Count > 0)
            {
                spawnedArea = Instantiate(prefabToInstantiate, hitResults[0].pose.position + new Vector3(0, 0.001f, 0), prefabToInstantiate.transform.rotation);
            }
            else { return; }

            //버튼 관련
            GameObject makeMapBtn = FindObject(canvas, "MapButton");
            GameObject selectBtn = FindObject(canvas, "MapSelectButton");
            GameObject TowerListPanel = FindObject(canvas, "TowerSpawnPanel");
            GameObject upgradeBtn = FindObject(canvas, "UpgradeTurretButton");

            if (makeMapBtn != null && selectBtn != null && TowerListPanel != null && upgradeBtn != null)
            {
                makeMapBtn.SetActive(false);
                selectBtn.SetActive(true);
                TowerListPanel.SetActive(false);
                upgradeBtn.SetActive(false);
            }
            else
            {
                Debug.Log("버튼을 찾을 수 없음");
            }

            // 1회성으로만 맵 생성
            isSpawnMap = true;
            map++;
        }
    }

    //버튼 찾기 메서드
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