using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WizzardTower : MonoBehaviour
{
    //번개관련
    public VisualEffect visualEffectPrefab; // 비주얼 이펙트 프리팹
    float spawnInterval = 0.3f; // 이펙트 생성 간격 (초)
    int spawnCount = 20; // 생성할 이펙트 개수
    float spawnRadius = 0.1f; // 중앙점으로부터의 최대 거리

    //실제 공격범위
    public GameObject attackRangePrefab;
    public GameObject attackRange;


    public bool isAttack = false;
    public bool seeAttackRange = false;

    //공격 가능 범위
    public GameObject attackRangeObject;

    private Camera mainCamera;

    //쿨타임 타워 색상관련
    private Renderer renderer;
    private MaterialPropertyBlock propBlock;

    void Start()
    {
        mainCamera = Camera.main;
        attackRangeObject = transform.Find("AttackRange").gameObject;

        renderer = GetComponentInChildren<Renderer>(); // Tower 오브젝트의 Renderer 컴포넌트
        propBlock = new MaterialPropertyBlock();
        StartCoroutine(IncreaseEmissionIntensityAndChangeColor(8, 20));
    }

    void Update()
    {
        if (isAttack && seeAttackRange)
        {
            RaycastHit hit;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("WizzardAttackRange")) // 'WizzardAttackRange' 태그를 가진 오브젝트에 닿았는지 확인
                {
                    // 닿은 오브젝트의 위치로 attackRange 위치 설정하지만, 최소 거리도 고려
                    setAttackPosition(hit.point);
                }
                else
                {
                    GameObject hitObject = hit.collider.gameObject;
                    // 레이를 hit한 오브젝트의 바로 아래로 발사 (예를 들어, y축으로 -1)
                    Ray downRay = new Ray(hitObject.transform.position, -Vector3.up);
                    RaycastHit[] hits = Physics.RaycastAll(downRay, Mathf.Infinity);
                    if (hits.Length == 0)
                    {
                        setAttackPosition(hit.point);
                    }
                    else
                    {
                        foreach (var floorHit in hits)
                        {
                            //태그가 WizzardAttackRange면
                            if (floorHit.collider.gameObject.CompareTag("WizzardAttackRange"))
                            {
                                setAttackPosition(hit.point);
                            }
                            else
                            {
                                setAttackPosition(hit.point);
                            }
                        }
                    }
                }

                void setAttackPosition(Vector3 targetPoint)
                {
                    if (attackRangeObject != null)
                    {
                        // 실린더의 원점
                        Vector3 cylinderCenter = attackRangeObject.transform.position;
                        float maxRadius = 0.35f; // 최대 반지름
                        float minRadius = 0.16f; // 최소 반지름

                        // 실린더의 원점에서 히트 포인트까지의 거리와 방향 계산
                        Vector3 directionFromCenter = (targetPoint - cylinderCenter);
                        directionFromCenter.y = 0; // y축 방향 무시
                        float distanceFromCenter = directionFromCenter.magnitude;

                        directionFromCenter = directionFromCenter.normalized; // 정규화된 방향 벡터

                        // 실린더 원 위의 가장 가까운 지점 계산
                        // 거리가 최소 반지름보다 작으면 최소 반지름 사용, 그렇지 않으면 최대 반지름 사용
                        float effectiveRadius = (distanceFromCenter < minRadius) ? minRadius : Mathf.Min(distanceFromCenter, maxRadius);
                        Vector3 nearestPointOnCircle = cylinderCenter + directionFromCenter * effectiveRadius;

                        Vector3 newPosition = new Vector3(nearestPointOnCircle.x, attackRangeObject.transform.position.y, nearestPointOnCircle.z);
                        attackRange.transform.position = newPosition + new Vector3(0, 0.005f, 0); ;
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 클릭 시
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (isAttack && seeAttackRange)
                {
                    isAttack = false;
                    seeAttackRange = false;

                    StartCoroutine(SpawnEffectsAt(attackRange.transform.position));
                    attackRangeObject.SetActive(false);

                    StartCoroutine(IncreaseEmissionIntensityAndChangeColor(8, 20));
                }
            }
        }
    }

    //공격 쿨타팀, 타워 색상
    public IEnumerator IncreaseEmissionIntensityAndChangeColor(float targetIntensity, float duration)
    {
        Color initialColor = new Color(255f / 255f, 13f / 255f, 0f / 255f);
        Color finalColor = new Color(0f / 255f, 15f / 255f, 191f / 255f);
        float time = 0;

        while (time < duration)
        {
            float newIntensity = Mathf.Lerp(0, targetIntensity, time / duration);
            renderer.GetPropertyBlock(propBlock);
            propBlock.SetColor("_EmissionColor", initialColor * newIntensity);
            renderer.SetPropertyBlock(propBlock);

            time += Time.deltaTime;
            yield return null;
        }

        float colorTransitionDuration = 2f;
        float colorTransitionTime = 0;

        while (colorTransitionTime < colorTransitionDuration)
        {
            float t = colorTransitionTime / colorTransitionDuration;
            Color currentColor = Color.Lerp(initialColor * targetIntensity, finalColor * targetIntensity, t);
            renderer.GetPropertyBlock(propBlock);
            propBlock.SetColor("_EmissionColor", currentColor);
            renderer.SetPropertyBlock(propBlock);

            colorTransitionTime += Time.deltaTime;
            yield return null;
        }

        renderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_EmissionColor", finalColor * targetIntensity);
        renderer.SetPropertyBlock(propBlock);

        isAttack = true;
    }

    //번개
    public IEnumerator SpawnEffectsAt(Vector3 center)
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 randomPos = Random.insideUnitCircle * spawnRadius;
            randomPos.z = randomPos.y;
            randomPos.y = 0; // 원하는 y값으로 조정
            randomPos += center;

            VisualEffect effectInstance = Instantiate(visualEffectPrefab, randomPos, Quaternion.identity);

            // 오브젝트의 AudioSource 컴포넌트를 가져온 후, 0.2초 딜레이로 재생 시작
            AudioSource audioSource = effectInstance.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                StartCoroutine(PlaySoundWithDelay(audioSource, 0.4f));
            }

            yield return new WaitForSeconds(spawnInterval); // 다음 이펙트 생성까지 대기
        }
        Destroy(attackRange);
        attackRangeObject.SetActive(false);
        seeAttackRange = false;
    }

    IEnumerator PlaySoundWithDelay(AudioSource audioSource, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.Play();
    }


    public void ToggleAttackRange()
    {
        if (attackRangeObject != null && isAttack) // AttackRange 오브젝트가 존재하는지 확인
        {
            Destroy(attackRange);
            attackRange = null;

            attackRangeObject.SetActive(!attackRangeObject.activeSelf); // 현재 활성화 상태의 반대로 설정
            seeAttackRange = attackRangeObject.activeSelf;

            if (seeAttackRange)
            {
                attackRange = Instantiate(attackRangePrefab, attackRangeObject.transform.position, Quaternion.identity);     
            }
        }
    }

    void OnEnable()
    {
        FindObjectOfType<WizzardTowerSystem>().RegisterWizzardTower(this);
    }

    void OnDisable()
    {
        FindObjectOfType<WizzardTowerSystem>().UnregisterWizzardTower(this);
    }
}