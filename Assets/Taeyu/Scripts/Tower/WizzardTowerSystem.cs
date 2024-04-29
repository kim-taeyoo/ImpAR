using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WizzardTowerSystem : MonoBehaviour
{
    private Camera mainCamera;
    private WizzardTower activeWizzardTower;
    // WizzardTower ��ü�� ������ ����Ʈ
    private List<WizzardTower> wizzardTowers = new List<WizzardTower>();

    public GameObject canvas;
    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // ��ġ �Է��� �ְ�, ù ��° ��ġ�� ���°� ȭ�鿡 ó�� ���� �������� Ȯ��
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // ù ��° ��ġ ���� ��������
            Touch touch = Input.GetTouch(0);

            // ��ġ ��ġ�κ��� Ray ����
            Ray ray = mainCamera.ScreenPointToRay(touch.position);
            RaycastHit hit;

            // Ray �߻�
            if (Physics.Raycast(ray, out hit))
            {
                WizzardTower wizzardTower = hit.collider.GetComponent<WizzardTower>();
                if (wizzardTower != null && wizzardTower.CompareTag("WizzardTower"))
                {
                    // �̹� Ȱ��ȭ�� WizzardTower�� �ְ�, �ٸ� WizzardTower�� Ŭ���Ǿ��ٸ�
                    if (activeWizzardTower != null && activeWizzardTower != wizzardTower)
                    {
                        // ���� WizzardTower�� ToggleAttackRange�� ȣ���Ͽ� ��Ȱ��ȭ
                        if (activeWizzardTower.seeAttackRange)
                        {
                            activeWizzardTower.ToggleAttackRange();
                        }
                    }

                    // ���� Ŭ���� WizzardTower�� Ȱ��ȭ�ϰ� ���� ������Ʈ
                    wizzardTower.ToggleAttackRange();
                    activeWizzardTower = wizzardTower;
                }
            }
        }

        UpdateAttackButtonStatus(); // AttackButton ���� ������Ʈ �޼��� ȣ��
    }

    // WizzardTower ��ü�� Ȱ��ȭ�Ǿ��� �� ȣ���� �޼���
    public void RegisterWizzardTower(WizzardTower tower)
    {
        if (!wizzardTowers.Contains(tower))
        {
            wizzardTowers.Add(tower);
        }
    }

    // WizzardTower ��ü�� ��Ȱ��ȭ�Ǿ��� �� ȣ���� �޼���
    public void UnregisterWizzardTower(WizzardTower tower)
    {
        if (wizzardTowers.Contains(tower))
        {
            wizzardTowers.Remove(tower);
        }
    }

    public void UpdateAttackButtonStatus()
    {
        GameObject AttackBtn = FindObject(canvas, "AttackButton");

        if (AttackBtn != null)
        {
            bool anyTowerAttacking = wizzardTowers.Exists(tower => tower.seeAttackRange);
            AttackBtn.SetActive(anyTowerAttacking);
        }
        else
        {
            Debug.Log("AttackButton is null");
        }
    }

    public void DoAttack()
    {
        if (activeWizzardTower == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // activeWizzardTower ������ ����Ͽ� �ش� ��ü�� ���¿� �޼��忡 ����
            if (activeWizzardTower.isAttack && activeWizzardTower.seeAttackRange)
            {
                activeWizzardTower.isAttack = false;
                activeWizzardTower.seeAttackRange = false;

                StartCoroutine(activeWizzardTower.SpawnEffectsAt(activeWizzardTower.attackRange.transform.position));
                activeWizzardTower.attackRangeObject.SetActive(false);

                StartCoroutine(activeWizzardTower.IncreaseEmissionIntensityAndChangeColor(8, 20));
            }
        }

        GameObject AttackBtn = FindObject(canvas, "AttackButton");

        if (AttackBtn != null)
        {
            AttackBtn.SetActive(false);
        }
        else
        {
            Debug.Log("AttackButton is null");
        }
    }

    GameObject FindObject(GameObject parent, string name)
    {
        if (parent.name == name) return parent;
        foreach (Transform child in parent.transform)
        {
            GameObject found = FindObject(child.gameObject, name);
            if (found != null) return found;
        }
        return null;
    }
}
