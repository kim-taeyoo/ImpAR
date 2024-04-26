using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

using UnityEngine.EventSystems;

[RequireComponent(typeof(ARRaycastManager))]
public class TapToPlaceMany : MonoBehaviour
{
    private ARRaycastManager raycastManager;



    [SerializeField] private GameObject prefabToInstantiate;
    
    private List<GameObject> spawnedObjects = new List<GameObject>();

    // stores the results of ARRaycast
    private List<ARRaycastHit> hitResults = new List<ARRaycastHit>();

    // Start is called before the first frame update
    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // if we touch the screen with one finger, do an AR Raycast to find
        // any AR planes under the finger
        if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            create();
        }
        // if we touch with two fingers, then delete all spawned objects
        else if(Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            delete();
        }
    }

    //return true if the given position is on top of UI
    bool isOverUI(Vector3 pos)
    {

        PointerEventData eventPosition = new PointerEventData(EventSystem.current);
        eventPosition.position = new Vector2(pos.x, pos.y);

        // make a raycast from the 2D position. 
        // if the raycast hits any UI at the posision, then we can return true
        // RaycastAll() finds any UI elements under the point
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventPosition, results);

        return results.Count > 0;   

    }

    void create()
    {
        // get a ray from the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 

        if(!isOverUI(Input.mousePosition) && raycastManager.Raycast(ray, hitResults, TrackableType.PlaneWithinPolygon))
        {
            // only get the first plane and instantiate on it
            Pose hitPose = hitResults[0].pose;
            GameObject go = Instantiate(prefabToInstantiate, hitPose.position + new Vector3(0, 0.2f, 0), hitPose.rotation);
            spawnedObjects.Add(go);
        }
    }

    public void delete()
    {
        foreach  (GameObject go in spawnedObjects)
        {
            Destroy(go);
        }
        spawnedObjects.Clear();
    }
}
