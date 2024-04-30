using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyActionController : MonoBehaviour
{
    private GameObject goal;
    private Vector3 goalPosition;
    private NavMeshAgent agent;
    private Collider c;

    private EnemyManager enemyManager;
    private EnemyAnimationController animationController;

    public int healthPoints = 10;
    public int attackPower = 2;

    public bool isDead = false; //적 죽는 함수가 너무 많이 실행돼서 한번만 실행되도록 bool 변수 만듦. //이규빈

    [SerializeField]
    private Transform rayPosition;

    public float attackDistance;

    // Start is called before the first frame update
    void Start()
    {
        isDead = false;
        agent = GetComponent<NavMeshAgent>();
        animationController = GetComponent<EnemyAnimationController>();
        c = GetComponent<Collider>();
        enemyManager = transform.parent.GetComponent<EnemyManager>();

        goal = enemyManager.SetGoal(); // 고정된 하나의 goal
        goalPosition = new Vector3(goal.transform.position.x,transform.position.y,goal.transform.position.z);
        
        //transform.LookAt(goalPosition);

        c.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (animationController.IsDeath)
        {
            EnemyDead();
            return;
        }

        if (goal == null)
        {
            animationController.DoRun(false);
            animationController.DoAttack(false);
            agent.destination = transform.position;
        }

        if (!animationController.IsSpawnning && c.enabled == false)
        {
            c.enabled = true;
        }

        if (goal != null && !animationController.IsSpawnning && !animationController.IsAttackAnimation)
        {
            agent.destination = goalPosition;
            if (agent.velocity.magnitude > 0.001)
            {
                animationController.DoRun(true);

            } else
            {
                animationController.DoRun(false);
            }
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackDistance);
        foreach (Collider c in hitColliders)
        {
            if (c.gameObject == goal && GoalCheckRay())
            {
                transform.LookAt(goalPosition);
                animationController.DoAttack(true);
                agent.destination = transform.position;
                animationController.IsAttackAnimation = true;
                break;
            } else
            {
                animationController.DoAttack(false);
            }
            
        }
    }

    //check there is no obstacles between enemy and goal
    bool GoalCheckRay()
    {
        Ray ray = new Ray(rayPosition.position,rayPosition.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackDistance))
        {
            if (hit.collider.gameObject == goal)
            {
                return true;
            }
        }
        return false;

    }

    private void EnemyDead() //적 죽음. 뭐임 이거 왜 ㅈㄴ많이 실행됨?? 디버그 찍어보면 너무 많이 실행되는데
    {
        if (!isDead)
        {
            agent.enabled = false;
            c.enabled = false;
            animationController.DoRun(false);
            animationController.DoAttack(false);
            animationController.DoHit(false);

            isDead = true;
        }
    }

    public void GetHit(int damage) //대미지 받음
    {
        healthPoints -= damage;

        if (!animationController.IsHitAnimation)
        {
            animationController.DoHit(true);
            animationController.IsHitAnimation = true;
        }

        if (!(healthPoints > 0))
        {
            animationController.DoDeath(true);
        }
    }

    public void AttackTower()
    {
        enemyManager.TargetDamage(attackPower);
    }

}
