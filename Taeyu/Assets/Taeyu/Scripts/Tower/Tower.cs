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

    private List<GameObject> enemiesInRange = new List<GameObject>();
    private GameObject currentTarget;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            enemiesInRange.Add(other.gameObject);
            UpdateTarget();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);
            currentTarget = null;
            UpdateTarget();
        }
    }

    private void UpdateTarget()
    {
        if (currentTarget != null)
        {
            return;
        }
        GameObject closeEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (GameObject enemy in enemiesInRange)
        {
            if(enemy == null)
            {
                return;
            }
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

    private void Update()
    {
        if(currentTarget != null)
        {
            Vector3 aimAt = new Vector3(currentTarget.transform.position.x, core.transform.position.y, currentTarget.transform.position.z);
            float distToTarget = Vector3.Distance(aimAt, gun.transform.position);

            Vector3 relativeTargetPosition = gun.transform.position + (gun.transform.forward * distToTarget);

            relativeTargetPosition = new Vector3(relativeTargetPosition.x, currentTarget.transform.position.y, relativeTargetPosition.z);

            gun.transform.rotation = Quaternion.Slerp(gun.transform.rotation, Quaternion.LookRotation(relativeTargetPosition - gun.transform.position), Time.deltaTime * turningSpeed);
            core.transform.rotation = Quaternion.Slerp(core.transform.rotation, Quaternion.LookRotation(aimAt - core.transform.position), Time.deltaTime * turningSpeed);

            Vector3 directonToTarget = currentTarget.transform.position - gun.transform.position;

            if(Vector3.Angle(directonToTarget, gun.transform.forward) < angleTurningAccuracy)
            {
                Fire();
            }
        }
    }

    public void EnemyDestroyed(GameObject enemy)
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
                projectile.GetComponent<Projectile>().SetDamage(10);
                projectile.GetComponent<Rigidbody>().velocity = firePoint1.forward * 24f;
                PlayFireSound();
            }
            if (turretLevel >= 2)
            {
                GameObject projectile = Instantiate(projectilePrefab, firePoint2.position, Quaternion.identity);
                projectile.GetComponent<Projectile>().SetDamage(10);
                projectile.GetComponent<Rigidbody>().velocity = firePoint2.forward * 24f;
                PlayFireSound();
            }
            if (turretLevel >= 3)
            {
                GameObject projectile = Instantiate(projectilePrefab, firePoint3.position, Quaternion.identity);
                projectile.GetComponent<Projectile>().SetDamage(10);
                projectile.GetComponent<Rigidbody>().velocity = firePoint3.forward * 24f;
                PlayFireSound();
            }
            if (turretLevel >= 4)
            {
                GameObject projectile = Instantiate(projectilePrefab, firePoint4.position, Quaternion.identity);
                projectile.GetComponent<Projectile>().SetDamage(10);
                projectile.GetComponent<Rigidbody>().velocity = firePoint4.forward * 24f;
                PlayFireSound();
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

    private IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(1.5f);
        isReloading = false;
    }
}
