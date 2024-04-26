using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePlane : MonoBehaviour
{
    public GameObject cubePrefab; // ť�� �������� �Ҵ��ϱ� ���� ����
    public float planeSize = 4f; // �÷����� ũ�� (10x10)
    public int cubesPerLine = 100; // �� �ٿ� �� ť���� ����

    void Start()
    {
        CreateCubes();
    }

    void CreateCubes()
    {
        // ť�� �ϳ��� ũ�� ���
        float cubeSize = planeSize / cubesPerLine;

        // ť�� �������� ������ ����
        cubePrefab.transform.localScale = new Vector3(cubeSize, cubeSize/10, cubeSize);

        for (int x = 0; x < cubesPerLine; x++)
        {
            for (int z = 0; z < cubesPerLine; z++)
            {
                // ť���� ��ġ ��� (�÷����� �߾��� �������� ���)
                float cubePositionX = ((x * cubeSize) - planeSize / 2 + cubeSize / 2) + transform.position.x;
                float cubePositionZ = ((z * cubeSize) - planeSize / 2 + cubeSize / 2) + transform.position.z;

                // ť�� �ν��Ͻ�ȭ
                GameObject cube = Instantiate(cubePrefab, new Vector3(cubePositionX, transform.position.y, cubePositionZ), Quaternion.identity);

                // ť�긦 �÷����� �ڽ����� ���� (������)
                cube.transform.parent = transform;
            }
        }
    }
}
