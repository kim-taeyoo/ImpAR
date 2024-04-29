using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class TouchManager : MonoBehaviour
{
    public GameObject skill;
    private ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public GameObject targetObj;
    public GameObject bomb;

    int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = transform.GetChild(0).GetChild(0).GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if(raycastManager.Raycast(ray, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            targetObj.transform.position = hits[0].pose.position;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (raycastManager.Raycast(touch.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    //Instantiate(skill, hits[0].pose.position, hits[0].pose.rotation);
                    if (count == 0)
                    {
                        Instantiate(skill, hits[0].pose.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
                        Instantiate(skill, hits[0].pose.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
                        Instantiate(skill, hits[0].pose.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
                        Instantiate(skill, hits[0].pose.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
                        Instantiate(skill, hits[0].pose.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
                        Instantiate(skill, hits[0].pose.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
                        Instantiate(skill, hits[0].pose.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
                        count++;
                    }
                    else
                    {
                        Instantiate(bomb, hits[0].pose.position + Vector3.up, Quaternion.identity);
                    }
                }
            }
        }
    }
}
