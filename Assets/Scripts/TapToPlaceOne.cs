using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class TapToPlaceOne : MonoBehaviour
{
    private ARRaycastManager raycastManager;

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
        // ARPlaneManager 컴포넌트를 가져옵니다.
        planeManager = GetComponent<ARPlaneManager>();
    }

    void Update()
    {
        if (isSpawnMap)
        {
            createAndUpdate();
        }
    }

   /* bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
        touchPosition = Vector2.zero;
        return false;
    }*/

    void createAndUpdate()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (raycastManager.Raycast(ray, hitResults, TrackableType.PlaneWithinPolygon) && hitResults.Count > 0)
        {
            // 오브젝트를 처음 생성할 때
            if (spawnedArea != null)
            {
                spawnedArea.transform.position = hitResults[0].pose.position + new Vector3(0, 0.001f, 0);
                // 카메라의 y축 회전 값을 가져옴
                yRotation = Camera.main.transform.eulerAngles.y;

                // 오브젝트의 회전을 카메라의 y축 회전 값으로 설정
                spawnedArea.transform.rotation = Quaternion.Euler(0, yRotation, 0);
            }
        }

        /* if (raycastManager.Raycast(ray, hitResults, TrackableType.PlaneWithinPolygon))
         {
             // 오브젝트를 처음 생성할 때
             if (spawnedObject == null)
             {
                 spawnedObject = Instantiate(prefabToInstantiate, hitResults[0].pose.position + new Vector3(0, 0.01f, 0),
                     hitResults[0].pose.rotation);

                 // ARPlaneManager 컴포넌트를 비활성화합니다.
                 if (planeManager != null)
                 {
                     planeManager.enabled = false;
                 }
             }
         }*/
    }


    public void isSelect()
    {
        if (isSpawnMap)
        {
            Destroy(spawnedArea);
            
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            if (raycastManager.Raycast(ray, hitResults, TrackableType.PlaneWithinPolygon) && hitResults.Count > 0)
            {

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
            }

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
        }
    }

    public void isMap()
    {
        if (!isSpawnMap && map < 1)
        {
            isSpawnMap = true;
            map++;

            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            // 카메라의 y축 회전 값을 가져옴
            yRotation = Camera.main.transform.eulerAngles.y;

            if (raycastManager.Raycast(ray, hitResults, TrackableType.PlaneWithinPolygon) && hitResults.Count > 0)
            {
                spawnedArea = Instantiate(prefabToInstantiate, hitResults[0].pose.position + new Vector3(0, 0.001f, 0), prefabToInstantiate.transform.rotation);
            }
        }
    }
}