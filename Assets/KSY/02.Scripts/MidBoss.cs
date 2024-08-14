using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MidBoss : MonoBehaviour, IAnimatorInterface
{

    public enum MidBossState
    {
        IDLE,
        MOVE,
        ATTACK,
        ATTACK_DELAY,
        DEAD
    }
    [Header("조절 가능")]
    [Tooltip("공격력")]
    public float attackPower;
    [Tooltip("공격 상태로 넘어가는 범위")]
    public float attackRange;
    [Tooltip("공격 상태로 전환된 후 다시 공격 가능할 때 까지 걸리는 시간")]
    public float attackDelay;

    [Header("터치 금지")]
    public MidBossState currState;
    
    WaitForSeconds attackDelays;

    EnemyHp enemyHp;
    EnemyAttackCanvas myAttackCanvas;
    GameObject myIndicator;
    public float indicatorDelay;
    FindPlayers findPlayers;

    NavMeshAgent agent;
    GameObject target;
    float distToTarget;

    Animator myAnim;

    private float currTime;

    float effTime = 0.1f;
    WaitForSeconds effTimeSec;
    Material myMaterial;
    Color orgColor;


    void Start()
    {
        enemyHp = GetComponent<EnemyHp>();
        enemyHp.onDie = OnDie;
        enemyHp.damageEff = DamageEff;
        myMaterial = GetComponentInChildren<SkinnedMeshRenderer>().material;
        effTimeSec = new WaitForSeconds(effTime);
        orgColor = myMaterial.color;

        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = attackRange;
        agent.updateRotation = false;
        findPlayers = GetComponent<FindPlayers>();
        myAttackCanvas = GetComponentInChildren<EnemyAttackCanvas>();
        myIndicator = myAttackCanvas.gameObject;
        myIndicator.SetActive(false);
        myAttackCanvas.attackPower = attackPower;
        myAttackCanvas.attackRange = attackRange;
        myAttackCanvas.attackCoolTime = indicatorDelay;
        myAnim = GetComponent<Animator>();
        ChangeState(MidBossState.IDLE);
    }

    void Update()
    {
        if (GameManager.instance.isStop) return;
        target = findPlayers.target;
        distToTarget = Vector3.Distance(transform.position, target.transform.position);
        

        switch (currState)
        {
            case MidBossState.IDLE:
                break;
            case MidBossState.MOVE:
                UpdateMove();
                break;
            case MidBossState.ATTACK:
                break;
            case MidBossState.ATTACK_DELAY:
                UpdateAttack_Delay();
                break;
            case MidBossState.DEAD:
                break;
            default:
                break;
        }
    }

    public void ChangeState(MidBossState state)
    {

        currTime = 0;
        currState = state;
        //agent.isStopped = true;
        agent.enabled = false;


        switch (currState)
        {
            case MidBossState.IDLE:
                OnIdle();
                break;
            case MidBossState.MOVE:
                myAnim.SetTrigger("MOVE");
                agent.enabled = true;
                //agent.isStopped = false;
                break;
            case MidBossState.ATTACK:
                OnAttack();
                
                break;
            case MidBossState.ATTACK_DELAY:
                myAnim.SetTrigger(currState.ToString());
                break;
            case MidBossState.DEAD:
                break;
            default:
                break;
        }
    }

    void OnIdle()
    {
        ChangeState(MidBossState.MOVE);
    }


    void UpdateMove()
    {
        agent.SetDestination(target.transform.position);
        Vector2 forward = new Vector2(transform.position.z, transform.position.x);
        Vector2 steeringTarget = new Vector2(agent.steeringTarget.z, agent.steeringTarget.x);

        // 방향을 구한 뒤, 역함수로 각을 구함.
        Vector2 dir = steeringTarget - forward;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // 방향적용
        transform.eulerAngles = Vector3.up * angle;
        if (distToTarget <= attackRange)
        {
            ChangeState(MidBossState.ATTACK);
        }
    }
    void UpdateAttack()
    {

    }

    void OnAttack()
    {
        Vector3 dir = target.transform.position - transform.position;
        transform.forward = (dir - Vector3.up * dir.y).normalized;
        ChangeState(MidBossState.ATTACK_DELAY);
        myAnim.SetTrigger("ATTACK");
        
    }
    void UpdateAttack_Delay()
    {
        if (IsDelayComplete(attackDelay))
        {
            DecideState();
        }
    }

    void DecideState() // 거리에 따른 상태 전환.
    {
        if(distToTarget <= attackRange)
        {
            ChangeState(MidBossState.ATTACK);
        }
        else
        {
            ChangeState(MidBossState.MOVE);
        }
    }

    bool IsDelayComplete(float delayTime) // 딜레이 시간
    {
        // 시간을 증가 시키자.
        currTime += Time.deltaTime;
        // 만약에 시간이 delayTime보다 커지면
        if (currTime >= delayTime)
        {
            // 현재시간 초기화
            currTime = 0;
            // true 반환
            return true;
        }
        // 그렇지 않으면
        else
        {
            // false 반환
            return false;
        }
    }

    void DamageEff()
    {
        StartCoroutine(C_DamageEff());
    }

    IEnumerator C_DamageEff()
    {
        myMaterial.color = Color.red;
        yield return effTimeSec;
        myMaterial.color = orgColor;
    }



    void OnDie()
    {
        H_PlayerManager.instance.SpawnExp(transform.position);
        Destroy(gameObject);
    }

    public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Attack"))
        {
            myIndicator.SetActive(true);
        }
    }

    public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Attack"))
        {
            
        }
    }
}
