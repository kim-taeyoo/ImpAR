using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;

public class EnemyManager : MonoBehaviour
{
    public GameManager gm;

    public GameObject WarriorPrefab;
    public GameObject ArcherPrefab;
    public GameObject MagePrefab;

    private List<GameObject> enemy = new List<GameObject>();

    /*
    void Update()
    {
        if (enemy.Count == 0) {  return; }
        foreach (GameObject e in enemy)
        {
            //make e to get goal
        }
    }
    */

    public GameObject SetGoal()
    {
        GameObject goal = gm.GetGoal();
        return goal;
    }

    private void WarriorSpawn(Vector3 originPos, float angle)
    {
        int enemyNum = Random.Range(1, 3);
        List<GameObject> temp = new List<GameObject>();

        Vector3 pos = new Vector3();
        pos.z = originPos.z + 1f;

        for (int i = 0; i < enemyNum; i++)
        {
            pos.x = originPos.x - 1f * (enemyNum/2) + i;
            GameObject e = Instantiate(WarriorPrefab, pos, Quaternion.identity);
            e.transform.parent = transform;

            e.transform.RotateAround(originPos, transform.up, -angle);

            temp.Add(e);
        }

        if (enemyNum%2 == 0)
        {
            foreach (GameObject e in temp)
            {
                e.transform.position += new Vector3(0, 0, 0.5f);
            }
        }

        enemy.AddRange(temp);
    }

    private void ArcherSpawn(Vector3 originPos, float angle)
    {
        int enemyNum = Random.Range(3, 6);
        List<GameObject> temp = new List<GameObject>();

        Vector3 pos = new Vector3();
        pos.z = originPos.z - 1f;

        for (int i = 0; i < enemyNum; i++)
        {
            pos.x = originPos.x - 1f * (enemyNum / 2) + i;
            GameObject e = Instantiate(ArcherPrefab, pos, Quaternion.identity);
            e.transform.parent = transform;

            e.transform.RotateAround(originPos, transform.up, -angle);

            temp.Add(e);
        }

        if (enemyNum % 2 == 0)
        {
            foreach (GameObject e in temp)
            {
                e.transform.position += new Vector3(0, 0, 0.5f);
            }
        }

        enemy.AddRange(temp);
    }

    private void MageSpawn(Vector3 originPos, float angle)
    {
        int enemyNum = Random.Range(2, 4);
        List<GameObject> temp = new List<GameObject>();

        Vector3 pos = new Vector3();
        pos.z = originPos.z;

        for (int i = 0; i < enemyNum; i++)
        {
            pos.x = originPos.x - 1f * (enemyNum / 2) + i;
            GameObject e = Instantiate(MagePrefab, pos, Quaternion.identity);
            e.transform.parent = transform;

            e.transform.RotateAround(originPos, transform.up, -angle);

            temp.Add(e);
        }

        if (enemyNum % 2 == 0)
        {
            foreach (GameObject e in temp)
            {
                e.transform.position += new Vector3(0, 0, 0.5f);
            }
        }

        enemy.AddRange(temp);
    }

    public void InstantiateEnemy(Vector3 pos, float angle)
    {
        WarriorSpawn(pos, angle);
        ArcherSpawn(pos, angle);
        MageSpawn(pos, angle);
    }

    public void EnemyDead(GameObject g)
    {
        foreach (GameObject e in enemy)
        {
            if (e == g)
            {
                enemy.Remove(e);
                Destroy(e, 3);
                break;
            }
        }
    }


}
