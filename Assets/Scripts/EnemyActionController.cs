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

    private EnemyAnimationController animationController;

    private int healthPoints = 10;

    [SerializeField]
    private Transform rayPosition;

    public float attackDistance;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animationController = GetComponent<EnemyAnimationController>();
        c = GetComponent<Collider>();
        c.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (animationController.IsDeath)
        {
            agent.enabled = false;
            ResetGoal();
            c.enabled = false;
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

        if (!animationController.IsSpawnning && c.enabled == false)
        {
            c.enabled = true;
        }

        if (goal != null && !animationController.IsSpawnning && !animationController.IsAttackAnimation)
        {
            agent.destination = goalPosition;
            animationController.DoRun(true);
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackDistance);
        foreach (Collider c in hitColliders)
        {
            if (c.gameObject == goal)
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

    bool GoalCheckRay()
    {
        Ray ray = new Ray(rayPosition.position,goalPosition);
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

    public void GetGoal(GameObject g)
    {
        goal = g;
        goalPosition = new Vector3(g.transform.position.x,transform.position.y, g.transform.position.z);
    }

    public void ResetGoal() { goal = null; }
}
