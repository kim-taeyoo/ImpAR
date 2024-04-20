using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class EnemyActionController : MonoBehaviour
{
    private GameObject goal;
    private Vector3 goalPosition;
    private NavMeshAgent agent;
    private Collider c;

    private EnemyAnimationController animationController;

    public int healthPoints = 10;

    [SerializeField]
    private Transform rayPosition;

    public float attackDistance;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animationController = GetComponent<EnemyAnimationController>();
        c = GetComponent<Collider>();

        goal = transform.parent.GetComponent<EnemyManager>().SetGoal(); // ������ �ϳ��� goal
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
            if (agent.velocity.magnitude > 0)
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

    private void EnemyDead()
    {
        agent.enabled = false;
        c.enabled = false;
        animationController.DoRun(false);
        animationController.DoAttack(false);
    }

    private void GetHit()
    {
        // get hit
    }

}
