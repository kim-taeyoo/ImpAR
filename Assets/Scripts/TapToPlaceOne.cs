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

    private GameObject spawnedObject;

    // stores the results of ARRaycast
    private List<ARRaycastHit> hitResults = new List<ARRaycastHit>();

    // ARPlaneManager ���� �߰�
    private ARPlaneManager planeManager;

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        // ARPlaneManager ������Ʈ�� �����ɴϴ�.
        planeManager = GetComponent<ARPlaneManager>();
    }

    void Update()
    {
        if (TryGetTouchPosition(out Vector2 touchPosition) && spawnedObject == null)
        {
            createAndUpdate();
        }
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
        touchPosition = Vector2.zero;
        return false;
    }

    void createAndUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

        if (raycastManager.Raycast(ray, hitResults, TrackableType.PlaneWithinPolygon))
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
        }
    }
}