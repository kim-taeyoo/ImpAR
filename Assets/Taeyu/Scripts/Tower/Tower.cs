using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public GameObject core; //좌우
    public GameObject gun; //상하
    public float turningSpeed = 10;
    public float angleTurningAccuracy = 80;

    public List<GameObject> enemiesInRange = new List<GameObject>();
    public GameObject currentTarget;

    public GameObject projectilePrefab;
    public Transform firePoint1;
    public Transform firePoint2;
    public Transform firePoint3;
    public Transform firePoint4;
    public int turretLevel;
    private bool isReloading = false;

    //오디오
    private AudioSource audioSource;
    public AudioClip fireSoundClip;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other) //적이 트리거에 닿으면 적 목록에 추가하는거
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            enemiesInRange.Add(other.gameObject);
            if (enemiesInRange.Count > 0) //범위 내에 들어온 다른 적이 있으면
            {
                UpdateTarget();
            }
        }
    }

    private void OnTriggerExit(Collider other) //적이 트리거에서 나가면 목록에서 제거
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);
            currentTarget = null;
            if (enemiesInRange.Count > 0) //범위 내에 들어온 다른 적이 있으면
            {
                UpdateTarget();
            }
        }
    }

    private void UpdateTarget() //현재 타겟이 설정이 안돼있으면 사정거리 안의 가장 가까운 적을 타겟팅
    {
        if (currentTarget != null && !currentTarget.GetComponent<EnemyAnimationController>().IsDeath)
        {
            return;
        }
        GameObject closeEnemy = null;
        float closestDistance = float.MaxValue;

        foreach(GameObject enemy in enemiesInRange)
        {
            if (enemy == null || (enemy != null && enemy.GetComponent<EnemyActionController>().isDead)) //적이 죽은 뒤에도 계속 쏘고 있어서 적이 죽으면 항목에서 삭제
            {
                enemiesInRange.Remove(enemy);
                break;
            }
        }

        foreach (GameObject enemy in enemiesInRange)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if(distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closeEnemy = enemy;
            }
        }

        if (closeEnemy != null)
        {
            currentTarget = closeEnemy;
        }
        else
        {
            currentTarget = null;
        }
    }

    private void Update() //머리 돌아가면서 타겟팅하는거
    {
        if(currentTarget != null)
        {
            if (currentTarget.GetComponent<EnemyActionController>().isDead) //만약 현재 타겟중인 놈이 죽었으면 바로 타겟 체인지
            {
                UpdateTarget();
            }
            else
            {

                Vector3 aimAt = new Vector3(currentTarget.transform.position.x, core.transform.position.y, currentTarget.transform.position.z);
                float distToTarget = Vector3.Distance(aimAt, gun.transform.position);

                Vector3 relativeTargetPosition = gun.transform.position + (gun.transform.forward * distToTarget);

                relativeTargetPosition = new Vector3(relativeTargetPosition.x, currentTarget.transform.position.y, relativeTargetPosition.z);

                gun.transform.rotation = Quaternion.Slerp(gun.transform.rotation, Quaternion.LookRotation(relativeTargetPosition - gun.transform.position), Time.deltaTime * turningSpeed);
                core.transform.rotation = Quaternion.Slerp(core.transform.rotation, Quaternion.LookRotation(aimAt - core.transform.position), Time.deltaTime * turningSpeed);

                Vector3 directonToTarget = currentTarget.transform.position - gun.transform.position;

                if (Vector3.Angle(directonToTarget, gun.transform.forward) < angleTurningAccuracy)
                {
                    Fire();
                }
            }
        }
        else //만약 타겟팅 하던 적이 사라지면
        {
            if(enemiesInRange.Count > 0) //범위 내에 들어온 다른 적이 있으면
            {
                Debug.Log("범위내에 다른 적 있어용");
                UpdateTarget(); 
            }
        }
    }

    public void EnemyDestroyed(GameObject enemy)  //이렇게 수동으로 삭제해주면, 두 개 이상의 타워가 타겟팅 하던 도중에 사라지면 어케함? 모든 타워에 EnemyDestroyed를 박아야하나
    {
        if(enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Remove(enemy);
            UpdateTarget();
        }
    }

    private void Fire() 
    {
        if (!isReloading)
        {
            if (turretLevel >= 1)
            {
                GameObject projectile = Instantiate(projectilePrefab, firePoint1.position, Quaternion.identity);
                projectile.GetComponent<Rigidbody>().velocity = firePoint1.forward * 24f;
                PlayFireSound();
                currentTarget.GetComponent<EnemyActionController>().GetHit(5);
            }
            if (turretLevel >= 2)
            {
                GameObject projectile = Instantiate(projectilePrefab, firePoint2.position, Quaternion.identity);
                projectile.GetComponent<Rigidbody>().velocity = firePoint2.forward * 24f;
                PlayFireSound();
                currentTarget.GetComponent<EnemyActionController>().GetHit(6);
            }
            if (turretLevel >= 3)
            {
                GameObject projectile = Instantiate(projectilePrefab, firePoint3.position, Quaternion.identity);
                projectile.GetComponent<Rigidbody>().velocity = firePoint3.forward * 24f;
                PlayFireSound();
                currentTarget.GetComponent<EnemyActionController>().GetHit(7);
            }
            if (turretLevel >= 4)
            {
                GameObject projectile = Instantiate(projectilePrefab, firePoint4.position, Quaternion.identity);
                projectile.GetComponent<Rigidbody>().velocity = firePoint4.forward * 24f;
                PlayFireSound();
                currentTarget.GetComponent<EnemyActionController>().GetHit(8);
            }
            StartCoroutine(Reload());
        }
    }

    // 사운드 재생 함수
    private void PlayFireSound()
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(fireSoundClip);
        }
    }

    private IEnumerator Reload() //1.5초마다 나감
    {
        isReloading = true;
        yield return new WaitForSeconds(1.5f);
        isReloading = false;
    }
}
