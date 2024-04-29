using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WizzardTower : MonoBehaviour
{
    //��������
    public VisualEffect visualEffectPrefab; // ���־� ����Ʈ ������
    float spawnInterval = 0.3f; // ����Ʈ ���� ���� (��)
    int spawnCount = 20; // ������ ����Ʈ ����
    float spawnRadius = 0.1f; // �߾������κ����� �ִ� �Ÿ�

    //���� ���ݹ���
    public GameObject attackRangePrefab;
    public GameObject attackRange;


    public bool isAttack = false;
    public bool seeAttackRange = false;

    //���� ���� ����
    public GameObject attackRangeObject;

    private Camera mainCamera;

    //��Ÿ�� Ÿ�� �������
    private Renderer renderer;
    private MaterialPropertyBlock propBlock;

    void Start()
    {
        mainCamera = Camera.main;
        attackRangeObject = transform.Find("AttackRange").gameObject;

        renderer = GetComponentInChildren<Renderer>(); // Tower ������Ʈ�� Renderer ������Ʈ
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
                if (hit.collider.CompareTag("WizzardAttackRange")) // 'WizzardAttackRange' �±׸� ���� ������Ʈ�� ��Ҵ��� Ȯ��
                {
                    // ���� ������Ʈ�� ��ġ�� attackRange ��ġ ����������, �ּ� �Ÿ��� ���
                    setAttackPosition(hit.point);
                }
                else
                {
                    GameObject hitObject = hit.collider.gameObject;
                    // ���̸� hit�� ������Ʈ�� �ٷ� �Ʒ��� �߻� (���� ���, y������ -1)
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
                            //�±װ� WizzardAttackRange��
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
                        // �Ǹ����� ����
                        Vector3 cylinderCenter = attackRangeObject.transform.position;
                        float maxRadius = 0.35f; // �ִ� ������
                        float minRadius = 0.16f; // �ּ� ������

                        // �Ǹ����� �������� ��Ʈ ����Ʈ������ �Ÿ��� ���� ���
                        Vector3 directionFromCenter = (targetPoint - cylinderCenter);
                        directionFromCenter.y = 0; // y�� ���� ����
                        float distanceFromCenter = directionFromCenter.magnitude;

                        directionFromCenter = directionFromCenter.normalized; // ����ȭ�� ���� ����

                        // �Ǹ��� �� ���� ���� ����� ���� ���
                        // �Ÿ��� �ּ� ���������� ������ �ּ� ������ ���, �׷��� ������ �ִ� ������ ���
                        float effectiveRadius = (distanceFromCenter < minRadius) ? minRadius : Mathf.Min(distanceFromCenter, maxRadius);
                        Vector3 nearestPointOnCircle = cylinderCenter + directionFromCenter * effectiveRadius;

                        Vector3 newPosition = new Vector3(nearestPointOnCircle.x, attackRangeObject.transform.position.y, nearestPointOnCircle.z);
                        attackRange.transform.position = newPosition + new Vector3(0, 0.005f, 0); ;
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(0)) // ���콺 ���� ��ư Ŭ�� ��
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

    //���� ��Ÿ��, Ÿ�� ����
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

    //����
    public IEnumerator SpawnEffectsAt(Vector3 center)
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 randomPos = Random.insideUnitCircle * spawnRadius;
            randomPos.z = randomPos.y;
            randomPos.y = 0; // ���ϴ� y������ ����
            randomPos += center;

            VisualEffect effectInstance = Instantiate(visualEffectPrefab, randomPos, Quaternion.identity);

            // ������Ʈ�� AudioSource ������Ʈ�� ������ ��, 0.2�� �����̷� ��� ����
            AudioSource audioSource = effectInstance.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                StartCoroutine(PlaySoundWithDelay(audioSource, 0.4f));
            }

            yield return new WaitForSeconds(spawnInterval); // ���� ����Ʈ �������� ���
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
        if (attackRangeObject != null && isAttack) // AttackRange ������Ʈ�� �����ϴ��� Ȯ��
        {
            Destroy(attackRange);
            attackRange = null;

            attackRangeObject.SetActive(!attackRangeObject.activeSelf); // ���� Ȱ��ȭ ������ �ݴ�� ����
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