using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WizzardTower : MonoBehaviour
{
    public VisualEffect visualEffectPrefab; // ���־� ����Ʈ ������
    public float spawnInterval = 1f; // ����Ʈ ���� ���� (��)
    public int spawnCount = 5; // ������ ����Ʈ ����
    public float spawnRadius = 0.1f; // �߾������κ����� �ִ� �Ÿ�

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ���콺 ���� ��ư Ŭ�� ��
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("AttackRange")) // 'AttackRange' �±׸� ���� ������Ʈ�� ��Ҵ��� Ȯ��
                {
                    StartCoroutine(SpawnEffectsAt(hit.point)); // ����Ʈ ���� �ڷ�ƾ ����
                }
            }
        }
    }

    IEnumerator SpawnEffectsAt(Vector3 center)
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 randomPos = Random.insideUnitCircle * spawnRadius;
            randomPos.z = randomPos.y;
            randomPos.y = 0; // y�� ����
            randomPos += center;

            Instantiate(visualEffectPrefab, randomPos, Quaternion.identity);

            yield return new WaitForSeconds(spawnInterval); // ���� ����Ʈ �������� ���
        }
    }
}
