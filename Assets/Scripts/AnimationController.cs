using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    Animator animator;
    int isRunningHash;
    int isAttackingHash;
    int isDeathHash;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isRunningHash = Animator.StringToHash("isRunning");
        isAttackingHash = Animator.StringToHash("isAttacking");
        isDeathHash = Animator.StringToHash("isDeath");
    }

    // Update is called once per frame
    void Update()
    {
        bool isRunning = animator.GetBool(isRunningHash);
        bool isAttacking = animator.GetBool(isAttackingHash);
        bool isDeath = animator.GetBool(isDeathHash);

        if(!isDeath && !isRunning && Input.GetKey("w"))
        {
            animator.SetBool(isRunningHash, true);
        }

        if(isRunning && !Input.GetKey("w"))
        {
            animator.SetBool(isRunningHash, false);
        }

        if (!isDeath && !isAttacking && Input.GetMouseButton(0))
        {
            animator.SetBool(isAttackingHash, true);
        }

        if (isAttacking && !Input.GetMouseButton(0))
        {
            animator.SetBool(isAttackingHash, false);
        }

        if(!isDeath && Input.GetMouseButton(1))
        {
            animator.SetBool(isDeathHash, true);
        }
    }

    void DestroyObject()
    {
        Destroy(gameObject, 3);
    }
}
