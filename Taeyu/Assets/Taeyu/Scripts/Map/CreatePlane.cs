using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CreatePlane : MonoBehaviour
{
    public GameObject cubePrefab;
    private float planeSize = 2f;
    private int cubesPerLine = 50;
    public Action onCreationComplete; // 완료 콜백을 위한 액션 추가

    void Start()
    {
        CreateCubes();
    }

    void CreateCubes()
    {
        float cubeSize = planeSize / cubesPerLine;
        cubePrefab.transform.localScale = new Vector3(cubeSize, cubeSize/10, cubeSize);

        for (int x = 0; x < cubesPerLine; x++)
        {
            for (int z = 0; z < cubesPerLine; z++)
            {
                float cubePositionX = ((x * cubeSize) - planeSize / 2 + cubeSize / 2) + transform.position.x;
                float cubePositionZ = ((z * cubeSize) - planeSize / 2 + cubeSize / 2) + transform.position.z;
                GameObject cube = Instantiate(cubePrefab, new Vector3(cubePositionX, transform.position.y, cubePositionZ), transform.rotation);
                cube.transform.parent = transform;
            }
        }
        
        // 큐브 생성이 완료된 후 콜백 호출
        onCreationComplete?.Invoke();
    }
}