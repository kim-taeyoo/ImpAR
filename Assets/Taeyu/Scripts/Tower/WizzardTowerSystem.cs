using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WizzardTowerSystem : MonoBehaviour
{
    private Camera mainCamera;
    private WizzardTower activeWizzardTower;
    // WizzardTower 객체를 저장할 리스트
    private List<WizzardTower> wizzardTowers = new List<WizzardTower>();

    public GameObject canvas;
    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (GameManager.gm != null && GameManager.gm.isEnemyTurn)
        {
            // 터치 입력이 있고, 첫 번째 터치의 상태가 화면에 처음 닿은 상태인지 확인
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                // 첫 번째 터치 정보 가져오기
                Touch touch = Input.GetTouch(0);

                // 터치 위치로부터 Ray 생성
                Ray ray = mainCamera.ScreenPointToRay(touch.position);
                RaycastHit hit;

                // Ray 발사
                if (Physics.Raycast(ray, out hit))
                {
                    WizzardTower wizzardTower = hit.collider.GetComponent<WizzardTower>();
                    if (wizzardTower != null && wizzardTower.CompareTag("WizzardTower"))
                    {
                        // 이미 활성화된 WizzardTower가 있고, 다른 WizzardTower가 클릭되었다면
                        if (activeWizzardTower != null && activeWizzardTower != wizzardTower)
                        {
                            // 이전 WizzardTower의 ToggleAttackRange를 호출하여 비활성화
                            if (activeWizzardTower.seeAttackRange)
                            {
                                activeWizzardTower.ToggleAttackRange();
                            }
                        }

                        // 새로 클릭된 WizzardTower를 활성화하고 참조 업데이트
                        wizzardTower.ToggleAttackRange();
                        activeWizzardTower = wizzardTower;
                    }
                }
            }
        }
        /*else
        {
            if (activeWizzardTower != null)
            {
                if (activeWizzardTower.seeAttackRange)
                {
                    activeWizzardTower.ToggleAttackRange();
                    activeWizzardTower = null;
                }
            }
        }*/

        UpdateAttackButtonStatus(); // AttackButton 상태 업데이트 메서드 호출
    }

    // WizzardTower 객체가 활성화되었을 때 호출할 메서드
    public void RegisterWizzardTower(WizzardTower tower)
    {
        if (!wizzardTowers.Contains(tower))
        {
            wizzardTowers.Add(tower);
        }
    }

    // WizzardTower 객체가 비활성화되었을 때 호출할 메서드
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
        if (activeWizzardTower == null) {
            Debug.Log("오류");
            return; 
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // activeWizzardTower 참조를 사용하여 해당 객체의 상태와 메서드에 접근
        if (activeWizzardTower.isAttack && activeWizzardTower.seeAttackRange)
        {
            StartCoroutine(activeWizzardTower.SpawnEffectsAt(activeWizzardTower.attackRange.transform.position));

            activeWizzardTower.isAttack = false;
            activeWizzardTower.seeAttackRange = false;
            activeWizzardTower.attackRangeObject.SetActive(false);

            StartCoroutine(activeWizzardTower.IncreaseEmissionIntensityAndChangeColor(8, 20));
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
