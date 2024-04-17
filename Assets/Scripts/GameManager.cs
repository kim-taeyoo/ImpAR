using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private GameObject goalPrefab;

    private GameObject enemy;
    private GameObject goal;

    // Start is called before the first frame update
    void Start()
    {
        goal = Instantiate(goalPrefab,new Vector3(20,2.5f,20),Quaternion.identity);
        enemy = Instantiate(enemyPrefab, new Vector3(20, 2.5f, 0), Quaternion.identity);

    }

    // Update is called once per frame
    void Update()
    {
        if (enemy == null) { return; }
        if (goal == null) { enemy.SendMessage("ResetGoal"); }
        enemy.SendMessage("GetGoal", goal);
     
    }
}
