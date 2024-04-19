using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private GameObject goalPrefab;

    public GameObject p; //parent

    private List<GameObject> enemy = new List<GameObject>();
    private GameObject goal;

    float enemySpawnTime = 5f;
    float firstSpawnTime = 1f;

    //For test
    private bool mousePressed;
    private Vector3 mousePosition;

    // Start is called before the first frame update
    void Start()
    {
        goal = Instantiate(goalPrefab, new Vector3(0, 2.5f, 0), Quaternion.identity);
        StartCoroutine(SpawnWaves());
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.Count == 0) { return; }
        if (goal == null)
        { foreach(GameObject e in enemy)
            e.SendMessage("ResetGoal");
        }
        foreach (GameObject e in enemy)
        {
            e.SendMessage("GetGoal", goal);
        }

        //For test
        if (Input.GetMouseButtonDown(0))
        {
            mousePressed = true;
            mousePosition = Input.mousePosition;
            //Debug.Log("mouse pressed");
        }

    }

    private void FixedUpdate()
    {
        // do raycast to see if we hit any shootable object
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        //Debug.DrawRay(ray.origin, ray.direction * 20, Color.red, 5f);

        if (mousePressed && Physics.Raycast(ray, out hit))
        {

            if (hit.collider.tag == "Enemy")
            {
                hit.collider.SendMessage("DoDeath",true);
            }

            mousePressed = false;

        }
    }

    private Vector3 RandomSpawnPostion(Vector3 originPos)
    {
        Vector3 pos = new Vector3();
        pos.x = originPos.x + Random.Range(-5f, 5f);
        pos.z = originPos.z + Random.Range(-5f, 5f);

        return pos;
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

    IEnumerator SpawnWaves()
    {
        while(true)
        {
            yield return new WaitForSeconds(firstSpawnTime);
            for (int i = 0; i < Random.Range(3, 5); ++i)
            {
                Vector3 pos = new Vector3(Random.Range(-30f, 30f), 2.5f, Random.Range(-30f, 30f));
                GameObject e = Instantiate(enemyPrefab, RandomSpawnPostion(pos), Quaternion.identity);
                e.transform.parent = p.transform;
                enemy.Add(e);

            }
            yield return new WaitForSeconds(enemySpawnTime);
        }
        
    }
}
