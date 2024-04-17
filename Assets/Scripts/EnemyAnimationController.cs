using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    Animator animator;
    int isRunningHash;
    int isAttackingHash;
    int isDeathHash;
    int isSpawnHash;

    bool isRunning;
    bool isAttacking;
    bool isDeath;
    bool isSpawnning;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isRunningHash = Animator.StringToHash("isRunning");
        isAttackingHash = Animator.StringToHash("isAttacking");
        isDeathHash = Animator.StringToHash("isDeath");
        isSpawnHash = Animator.StringToHash("isSpawnning");
    }

    // Update is called once per frame
    void Update()
    {
        isRunning = animator.GetBool(isRunningHash);
        isAttacking = animator.GetBool(isAttackingHash);
        isDeath = animator.GetBool(isDeathHash);
        isSpawnning = animator.GetBool(isSpawnHash);

        if (!isDeath && Input.GetMouseButton(1))
        {
            DoDeath(true);
        }
        
    }

    public void DoRun(bool b)
    {
        animator.SetBool(isRunningHash, b);
    }

    public void DoAttack(bool b)
    {
        animator.SetBool(isAttackingHash, b);
    }

    public void DoDeath(bool b)
    {
        animator.SetBool(isDeathHash, b);
    }

    public bool IsRunning { get { return isRunning; } }
    public bool IsAttacking { get { return isAttacking; } }
    public bool IsDeath { get { return isDeath; } }
    public bool IsSpawnning { get { return isSpawnning; } }
}