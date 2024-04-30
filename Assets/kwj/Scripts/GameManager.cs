using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject goal;

    private Collider plane;

    public EnemyManager enemyManager;

    private Vector3 planePosition;

    public bool enemySpawn = false;

    float enemySpawnTime = 10f;
    float firstSpawnTime = 1f;

    //For test
    private bool mousePressed;
    private Vector3 mousePosition;
    
    // 이규빈 작성
    public static GameManager gm; //static 게임매니저
    public int stage; //몇번째 스테이지인지
    public int money; //가지고 있는 돈
    public int enemyNum; //이번 스테이지에서 남아있는 적의 수
    public int wave; //몇번째 웨이브인지 (현재는 스테이지 수만큼 반복할 예정)
    public bool isEnemyTurn = false; //적 턴인지 (true면 타워 개설 불가)
    public int timer;

    private void Awake()
    {
        if (gm == null)
        {
            gm = gameObject.GetComponent<GameManager>();
            stage = 1;
            money = 1000;
            enemyNum = 0;
            wave = 0;
            StartCoroutine(StartTimer(50));
        }
    }


    public void StartSpawn()  //스테이지의 시작
    {
        isEnemyTurn = true;
        enemySpawn = true;
        StartCoroutine(SpawnWaves());
    }
    public void StopSpawn()  //소환 중지 (웨이브가 다 됨)
    {
        enemySpawn = false;
        StopCoroutine(SpawnWaves());
    }
    public void PlayerTurn()  //이번 스테이지의 적을 모두 해치웠을 때 호출됨
    {
        isEnemyTurn = false;
        wave = 0;
        stage++;
        StartCoroutine(StartTimer(5));
    }

    IEnumerator StartTimer(int startTimer)
    {
        timer = startTimer;
        while(timer > 0)
        {
            yield return new WaitForSeconds(1);
            timer--;
            UIManager.um.changeTimer(timer);
        }
        StartSpawn();
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

        if (Input.GetKeyDown(KeyCode.G))
        {
            timer = 2;
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

    public void SetGoal(GameObject g)
    {
        goal = g;
    }

    public GameObject GetGoal()
    {
        return goal;
    }

    public void SetPos(Vector3 pos)
    {
        planePosition = pos;
    }

    public void SetPlane(Collider p)
    {
        plane = p;
    }

    public void GoalDamaged(int damage)
    {
        //goal got damage
    }

    
    IEnumerator SpawnWaves()
    {
        while(enemySpawn)
        {
            yield return new WaitForSeconds(firstSpawnTime);
            for (int i = 0; i < Random.Range(1, 3); ++i)
            {
                float range_X = plane.bounds.size.x / 2;
                float range_Z = plane.bounds.size.z / 2;
                float range = Mathf.Min(range_X, range_Z);

                int deg = Random.Range(0, 360);
                float rad = range - 0.08f;

                float x = Mathf.Cos(deg * Mathf.Deg2Rad) * rad;
                float z = Mathf.Sin(deg * Mathf.Deg2Rad) * rad;

                Vector3 pos = planePosition +  new Vector3(x, 0.0f, z);
                //Debug.Log(pos);
                Vector3 targetDir = goal.transform.position - pos;
                float angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);

                Debug.Log("Spwan");
                enemyManager.InstantiateEnemy(pos, angle);
            }
            wave++;
            if(wave >= stage)
            {
                StopSpawn();
            }
            yield return new WaitForSeconds(enemySpawnTime);
        }
        
    }

 
}
