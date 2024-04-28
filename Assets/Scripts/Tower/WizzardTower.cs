using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WizzardTower : MonoBehaviour
{
    public VisualEffect visualEffectPrefab; // 비주얼 이펙트 프리팹
    public float spawnInterval = 1f; // 이펙트 생성 간격 (초)
    public int spawnCount = 5; // 생성할 이펙트 개수
    public float spawnRadius = 0.1f; // 중앙점으로부터의 최대 거리

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 클릭 시
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("AttackRange")) // 'AttackRange' 태그를 가진 오브젝트에 닿았는지 확인
                {
                    StartCoroutine(SpawnEffectsAt(hit.point)); // 이펙트 생성 코루틴 시작
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
            randomPos.y = 0; // y값 고정
            randomPos += center;

            Instantiate(visualEffectPrefab, randomPos, Quaternion.identity);

            yield return new WaitForSeconds(spawnInterval); // 다음 이펙트 생성까지 대기
        }
    }
}
