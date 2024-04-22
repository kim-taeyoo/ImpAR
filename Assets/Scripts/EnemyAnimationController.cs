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
    int isHitHash;

    bool isRunning;
    bool isAttacking;
    bool isDeath;
    bool isSpawnning;
    bool isHit;

    bool attackAnimation;
    bool hitAnimation;
    float temp = 0.7f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isRunningHash = Animator.StringToHash("isRunning");
        isAttackingHash = Animator.StringToHash("isAttacking");
        isDeathHash = Animator.StringToHash("isDeath");
        isSpawnHash = Animator.StringToHash("isSpawnning");
        isHitHash = Animator.StringToHash("isHit");

    }

    // Update is called once per frame
    void Update()
    {
        isRunning = animator.GetBool(isRunningHash);
        isAttacking = animator.GetBool(isAttackingHash);
        isDeath = animator.GetBool(isDeathHash);
        isSpawnning = animator.GetBool(isSpawnHash);
        isHit = animator.GetBool(isHitHash);

        if (animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 0.5f)
        {
            if (temp >= 0)
            {
                temp -= Time.deltaTime;
            }
            animator.SetLayerWeight(1, temp);
            
        }

        if (temp < 0)
        {
            temp = 0.7f;
        }

    }

    public void AnimationObserver(string message)
    {
        if (message.Equals("SpawnEnded"))
        {
            animator.SetBool(isSpawnHash, false);
        }

        if (message.Equals("Attacked"))
        {
            //Tower got damage
            Debug.Log("Tower get damage");
        }

        if (message.Equals("AttackEnded"))
        {
            DoAttack(false);
            attackAnimation = false;
        }

        if (message.Equals("HitEnded"))
        {
            DoHit(false);
            hitAnimation = false;
        }

        if (message.Equals("DeathEnded"))
        {
            //Debug.Log(gm);
            transform.parent.GetComponent<EnemyManager>().EnemyDead(animator.gameObject);
            Debug.Log(gameObject.name + "said dead");
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

    public void DoHit(bool b)
    {
        animator.SetBool(isHitHash, b);
    }

    public void DoDeath(bool b)
    {
        animator.SetBool(isDeathHash, b);
    }

    public bool IsRunning { get { return isRunning; } }
    public bool IsAttacking { get { return isAttacking; } }
    public bool IsDeath { get { return isDeath; } }
    public bool IsSpawnning { get { return isSpawnning; } }
    public bool IsAttackAnimation { get { return attackAnimation; } set { attackAnimation = value;} }
    public bool IsHitAnimation { get { return hitAnimation; } set { hitAnimation = value; } }

}
