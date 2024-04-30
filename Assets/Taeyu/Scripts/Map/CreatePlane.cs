using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CreatePlane : MonoBehaviour
{
    public GameObject cubePrefab;
    private float planeSize = 2f;
    private int cubesPerLine = 50;
    public Action onCreationComplete; // �Ϸ� �ݹ��� ���� �׼� �߰�

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
                GameObject cube = Instantiate(cubePrefab, new Vector3(cubePositionX, transform.position.y, cubePositionZ), Quaternion.identity);
                cube.transform.parent = transform;
                
                // �� ť�� �������� ���̸� ���� ������Ʈ�� �ִ��� Ȯ��
                CheckForObjectAbove(cube);
            }
        }
        
        // ť�� ������ �Ϸ�� �� �ݹ� ȣ��
        onCreationComplete?.Invoke();
    }

    void CheckForObjectAbove(GameObject cube)
    {
        Ray ray = new Ray(cube.transform.position, Vector3.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // ���̿� ���� ������Ʈ�� �ְ� "Plane" �±װ� �ƴϸ�
            if (!hit.collider.CompareTag("Plane"))
            {
                // �ش� ť���� �±׸� "Occupied"�� ����
                cube.tag = "Occupied";
            }
        }
    }
}