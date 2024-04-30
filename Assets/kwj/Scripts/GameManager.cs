using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject goal;

    public Collider plane;

    public EnemyManager enemyManager;

    private Vector3 planePosition;

    float enemySpawnTime = 10f;
    float firstSpawnTime = 1f;

    //For test
    private bool mousePressed;
    private Vector3 mousePosition;

    
    // 이규빈 작성
    public static GameManager gm; //static 게임매니저
    int stage; //몇번째 스테이지인지
    int money; //가지고 있는 돈
    int enemyNum; //이번 스테이지에서 남아있는 적의 수

    void Start()
    {
        if(gm == null)
        {
            gm = gameObject.GetComponent<GameManager>();
            stage = 1;
            money = 1000;
        }
        StartCoroutine(SpawnWaves());
        planePosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
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
            //Debug.Log(hit.collider.name);
            if (hit.collider.tag == "Enemy")
            {
                hit.collider.GetComponent<EnemyActionController>().GetHit(1);
            }

            mousePressed = false;

        }
    }

    public GameObject GetGoal()
    {
        return goal;
    }

    public void GoalDamaged(int damage)
    {
        //goal got damage
    }

    
    IEnumerator SpawnWaves()
    {
        while(true)
        {
            yield return new WaitForSeconds(firstSpawnTime);
            for (int i = 0; i < Random.Range(1, 3); ++i)
            {
                   float range_X = plane.bounds.size.x / 2;
                   float range_Z = plane.bounds.size.z / 2;
                   float range = Mathf.Min(range_X, range_Z);

                   int deg = Random.Range(0, 360);
                   float rad = Random.Range(range - 0.08f, range-0.04f);

                   float x = Mathf.Cos(deg * Mathf.Deg2Rad) * rad;
                   float z = Mathf.Sin(deg * Mathf.Deg2Rad) * rad;

                   Vector3 pos = planePosition +  new Vector3(x, 0.0f, z);
                   Vector3 targetDir = goal.transform.position - pos;
                   float angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);
                

                enemyManager.InstantiateEnemy(pos, angle);
            }
            yield return new WaitForSeconds(enemySpawnTime);
        }
        
    }

 
}
