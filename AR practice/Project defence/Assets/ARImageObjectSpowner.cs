using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARImageObjectSpowner : MonoBehaviour
{

    private ARTrackedImageManager imgManager;

    [SerializeField]
    private GameObject prefab;
    private GameObject spawned;

    // Start is called before the first frame update
    void Start()
    {
        imgManager = GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        imgManager.trackImagesChanged += OnImageChanged;


    }

    private void OnDisable()
    {

    }

    private void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (ARTrackedImage img in args.added)
        {
            Debug.Log("Image added: " + img.referenceImage.name);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
