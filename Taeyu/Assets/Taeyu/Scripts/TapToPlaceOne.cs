using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class TapToPlaceOne : MonoBehaviour
{
    public GameObject canvas;

    private ARRaycastManager raycastManager;

    //���ÿ���ǥ�� && ������ �ٴ�
    [SerializeField] private GameObject prefabToInstantiate;
    [SerializeField] private GameObject prefabToPlaneInstantiate;

    private GameObject spawnedArea;

    // stores the results of ARRaycast
    private List<ARRaycastHit> hitResults = new List<ARRaycastHit>();

    // ARPlaneManager ���� �߰�
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
            // ������Ʈ�� ó�� ������ ��
            if (spawnedArea != null)
            {
                spawnedArea.transform.position = hitResults[0].pose.position + new Vector3(0, 0.001f, 0);
                // ī�޶��� y�� ȸ�� ���� ������
                yRotation = Camera.main.transform.eulerAngles.y;

                // ������Ʈ�� ȸ���� ī�޶��� y�� ȸ�� ������ ����
                spawnedArea.transform.rotation = Quaternion.Euler(0, yRotation, 0);
            }
        }

        /* if (raycastManager.Raycast(ray, hitResults, TrackableType.PlaneWithinPolygon))
         {
             // ������Ʈ�� ó�� ������ ��
             if (spawnedObject == null)
             {
                 spawnedObject = Instantiate(prefabToInstantiate, hitResults[0].pose.position + new Vector3(0, 0.01f, 0),
                     hitResults[0].pose.rotation);

                 // ARPlaneManager ������Ʈ�� ��Ȱ��ȭ�մϴ�.
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
                // CreatePlane ��ũ��Ʈ�� �����Ͽ� �ݹ� ����
                CreatePlane createPlaneScript = spawnedArea.GetComponent<CreatePlane>();
                if (createPlaneScript != null)
                {
                    createPlaneScript.onCreationComplete = () =>
                    {
                        // ť�� ������ �Ϸ�� �� ȸ�� ����
                        spawnedArea.transform.rotation = Quaternion.Euler(0, yRotation, 0);
                    };
                }

                isSpawnMap = false;
            }

            // ARPlaneManager ������Ʈ�� ��Ȱ��ȭ�մϴ�.
            if (planeManager != null)
            {
                planeManager.enabled = false;
                foreach (var plane in planeManager.trackables)
                {
                    Debug.Log(plane);
                    plane.gameObject.SetActive(false);
                }
            }

            //��ư ����
            GameObject makeMapBtn = FindObject(canvas, "MapButton");
            GameObject selectBtn = FindObject(canvas, "MapSelectButton");
            GameObject TowerListBtn = FindObject(canvas, "SpawnTurretButton");
            GameObject upgradeBtn = FindObject(canvas, "UpgradeTurretButton");

            if (makeMapBtn != null && selectBtn != null && TowerListBtn != null && upgradeBtn != null)
            {
                makeMapBtn.SetActive(false);
                selectBtn.SetActive(false);
                TowerListBtn.SetActive(true);
                upgradeBtn.SetActive(true);
            }
            else
            {
                Debug.Log("��ư�� ã�� �� ����");
            }
        }
    }

    public void isMap()
    {
        if (!isSpawnMap && map < 1)
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            // ī�޶��� y�� ȸ�� ���� ������
            yRotation = Camera.main.transform.eulerAngles.y;

            if (raycastManager.Raycast(ray, hitResults, TrackableType.PlaneWithinPolygon) && hitResults.Count > 0)
            {             
            spawnedArea = Instantiate(prefabToInstantiate, hitResults[0].pose.position + new Vector3(0, 0.001f, 0), prefabToInstantiate.transform.rotation);
            }
            else { return; }

            //��ư ����
            GameObject makeMapBtn = FindObject(canvas, "MapButton");
            GameObject selectBtn = FindObject(canvas, "MapSelectButton");
            GameObject TowerListBtn = FindObject(canvas, "SpawnTurretButton");
            GameObject upgradeBtn = FindObject(canvas, "UpgradeTurretButton");

            if (makeMapBtn != null && selectBtn != null && TowerListBtn != null && upgradeBtn != null)
            {
                makeMapBtn.SetActive(false);
                selectBtn.SetActive(true);
                TowerListBtn.SetActive(false);
                upgradeBtn.SetActive(false);
            }
            else
            {
                Debug.Log("��ư�� ã�� �� ����");
            }

                // 1ȸ�����θ� �� ����
                isSpawnMap = true;
            map++;
        }
    }

    //��ư ã�� �޼���
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