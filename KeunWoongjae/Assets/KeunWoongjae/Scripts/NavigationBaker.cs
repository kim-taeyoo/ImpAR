using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class NavigationBaker : MonoBehaviour
{
    [SerializeField]
    private NavMeshSurface plane;

    // Start is called before the first frame update
    void Start()
    {
        plane.BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
        plane.BuildNavMesh();
    }
}
