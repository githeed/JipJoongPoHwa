using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MidBoss : MonoBehaviour
{

    public enum MidBossState
    {
        IDLE,
        MOVE,
        ATTACK,
        ATTACK_DELAY,
        DEAD
    }

    public MidBossState currState;

    public float attackDelay;
    WaitForSeconds attackDelays;
    public float attackPower;
    public float attackRange;

    EnemyHp enemyHp;
    EnemyAttackCanvas myAttackCanvas;
    GameObject myIndicator;

    NavMeshAgent agent;
    GameObject target;
    float distToTarget;

    private float currTime;

    void Start()
    {
        enemyHp = GetComponent<EnemyHp>();
        enemyHp.onDie = OnDie;

        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = attackRange;

        myAttackCanvas = GetComponentInChildren<EnemyAttackCanvas>();
        myIndicator = myAttackCanvas.gameObject;
        myIndicator.SetActive(false);
        myAttackCanvas.attackPower = attackPower;
        myAttackCanvas.attackRange = attackRange;
        myAttackCanvas.attackCoolTime = attackDelay;
        ChangeState(MidBossState.IDLE);
    }

    void Update()
    {
        target = GetComponent<FindPlayers>().target;
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

        print(currState + " ----> " + state);
        currTime = 0;
        currState = state;
        agent.isStopped = true;


        switch (currState)
        {
            case MidBossState.IDLE:
                OnIdle();
                break;
            case MidBossState.MOVE:
                agent.isStopped = false;
                break;
            case MidBossState.ATTACK:
                OnAttack();
                break;
            case MidBossState.ATTACK_DELAY:
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
        print("이동중");
        if(distToTarget <= attackRange)
        {
            ChangeState(MidBossState.ATTACK);
        }
    }
    void UpdateAttack()
    {

    }

    void OnAttack()
    {
        myIndicator.SetActive(true);
        ChangeState(MidBossState.ATTACK_DELAY);
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

    void OnDie()
    {
        Destroy(gameObject);
    }
}
