using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyActionController : MonoBehaviour
{
    private GameObject goal;
    private NavMeshAgent agent;

    private EnemyAnimationController animationController;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animationController = GetComponent<EnemyAnimationController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (animationController.IsDeath)
        {
            agent.speed = 0;
            animationController.DoRun(false);
            animationController.DoAttack(false);
            return;
        }

        if (goal == null)
        {
            animationController.DoRun(false);
            animationController.DoAttack(false);
            agent.destination = transform.position;
        }


        if (goal != null && !animationController.IsSpawnning && !animationController.IsAttacking)
        {
            agent.destination = goal.transform.position;
            agent.speed = 5;
            animationController.DoRun(true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject == goal)
        {
            transform.LookAt(agent.destination);
            animationController.DoAttack(true);
            agent.speed = 1;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject == goal)
        {
            animationController.DoAttack(false);
        }
    }


    public void GetGoal(GameObject g)
    {
        goal = g;
    }

    public void ResetGoal() { goal = null; }
}
