using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyActionController : MonoBehaviour
{
    private GameObject goal;
    private NavMeshAgent agent;
    private Rigidbody rb;

    private EnemyAnimationController animationController;

    private int healthPoints = 10;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animationController = GetComponent<EnemyAnimationController>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (animationController.IsDeath)
        {
            agent.enabled = false;
            rb.isKinematic = true;
            ResetGoal();
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


        if (goal != null && !animationController.IsSpawnning && !animationController.IsAttackAnimation)
        {
            agent.destination = goal.transform.position;
            agent.speed = 5;
            animationController.DoRun(true);
        }
    }

    void GetHit(int damage)
    {
        healthPoints -= damage;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == goal)
        {
            transform.LookAt(agent.destination);
            animationController.DoAttack(true);
            agent.speed = 0;
            animationController.IsAttackAnimation = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == goal)
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
