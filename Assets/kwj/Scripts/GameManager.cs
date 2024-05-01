using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject goal;

    public Collider plane;

    public EnemyManager enemyManager;

    private Vector3 planePosition;

    public bool enemySpawn = false;

    float enemySpawnTime = 10f;
    float firstSpawnTime = 1f;

    public GameObject spawnPoints;
    public List<Vector3> spawnPos = new List<Vector3>();

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
            UIManager.um.ChangeMoneyNum(0, 1000); //UI의 돈 숫자가 올라가는 애니메이션 실행
            enemyNum = 0;
            UIManager.um.changeEnemyNum(); //UI의 적 숫자 변경
            wave = 0;
            StartCoroutine(StartTimer(50)); //G를 눌러서 남은 시간 무시하고 바로 시작 가능

            GetEnemySpawnPoints();
        }
    }


    public void StartSpawn()  //스테이지의 시작
    {
        isEnemyTurn = true; //적 턴중으로 표시
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
        isEnemyTurn = false; //적 턴이 끝남을 표시
        wave = 0;
        stage++;
        UIManager.um.ChangeStageNum(stage);
        UIManager.um.ClearStageAnim(); //스테이지 클리어 애니메이션 작동
        StartCoroutine(StartTimer(5)); //인자로 넣어주는 숫자 초만큼 기다린 후 다음 스테이지를 시작한다.
    }

    IEnumerator StartTimer(int startTimer) //타이머를 작동시키는 코루틴 함수. 매개변수로 받아온 시간만큼 기다린 후 다음 적 턴 실행
    {  //참고로 G를 눌러서 기다리는 시간을 스킵할 수 있다. (디버그용)
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

    public GameObject GetGoal()
    {
        return goal;
    }

    public void GoalDamaged(int damage)
    {
        //goal got damage
    }

    private void GetEnemySpawnPoints()
    {
        for (int i = 0; i < 16; i++)
        {
            spawnPos.Add(spawnPoints.transform.GetChild(i).transform.position);
        }
    }
    
    IEnumerator SpawnWaves()
    {
        while(enemySpawn)
        {
            yield return new WaitForSeconds(firstSpawnTime);
            for (int i = 0; i < Random.Range(1, 3); ++i)
            {
                int temp = Random.Range(0, 15);

                Vector3 pos = planePosition + spawnPos[temp];
                
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
