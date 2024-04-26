using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePlane : MonoBehaviour
{
    public GameObject cubePrefab; // 큐브 프리팹을 할당하기 위한 변수
    public float planeSize = 4f; // 플레인의 크기 (10x10)
    public int cubesPerLine = 100; // 한 줄에 들어갈 큐브의 개수

    void Start()
    {
        CreateCubes();
    }

    void CreateCubes()
    {
        // 큐브 하나당 크기 계산
        float cubeSize = planeSize / cubesPerLine;

        // 큐브 프리팹의 스케일 조정
        cubePrefab.transform.localScale = new Vector3(cubeSize, cubeSize/10, cubeSize);

        for (int x = 0; x < cubesPerLine; x++)
        {
            for (int z = 0; z < cubesPerLine; z++)
            {
                // 큐브의 위치 계산 (플레인의 중앙을 기준으로 계산)
                float cubePositionX = ((x * cubeSize) - planeSize / 2 + cubeSize / 2) + transform.position.x;
                float cubePositionZ = ((z * cubeSize) - planeSize / 2 + cubeSize / 2) + transform.position.z;

                // 큐브 인스턴스화
                GameObject cube = Instantiate(cubePrefab, new Vector3(cubePositionX, transform.position.y, cubePositionZ), Quaternion.identity);

                // 큐브를 플레인의 자식으로 설정 (선택적)
                cube.transform.parent = transform;
            }
        }
    }
}
