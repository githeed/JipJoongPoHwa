using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    public enum BossState
    {
        IDLE,
        MOVE,
        ATTACK,
        ATTACK_DELAY,
        DEAD
    }
    public GameObject target;
    EnemyHp enemyHp;
    FindPlayers findPlayers;
    NavMeshAgent agent;

    Vector3 attackPos;



    public BossState currState;
    Animator myAnim;
    float toTargetDist;
    Vector3 toTargetDir;

    public float attackRange;
    public float moveSpeed;

    void Start()
    {
        enemyHp = GetComponent<EnemyHp>();
        enemyHp.onDie = OnDie;
        findPlayers = GetComponent<FindPlayers>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        myAnim = GetComponent<Animator>();
        ChangeState(BossState.MOVE);
        agent.updateRotation = false;
    }

    void Update()
    {
        target = findPlayers.target;
        if (agent.enabled) MyRotate();
        toTargetDir = target.transform.position - transform.position;
        toTargetDist = toTargetDir.magnitude;


        switch (currState)
        {
            case BossState.IDLE:
                break;
            case BossState.MOVE:
                UpdateMove();
                break;
            case BossState.ATTACK:
                break;
            case BossState.ATTACK_DELAY:
                UpdateAttack_Delay();
                break;
            case BossState.DEAD:
                break;
            default:
                break;
        }

    }

    void ChangeState(BossState state)
    {
        print(currState + " ----> " + state);
        currState = state;
        myAnim.SetBool("MOVE", false);
        agent.enabled = false;
        currTime = 0;

        switch (currState)
        {
            case BossState.IDLE:
                break;
            case BossState.MOVE:
                agent.enabled = true;
                myAnim.SetBool("MOVE", true);
                break;
            case BossState.ATTACK:
                OnDecideAttackPattern();
                break;
            case BossState.ATTACK_DELAY:
                break;
            case BossState.DEAD:
                break;
            default:
                break;
        }
    }

    void UpdateMove()
    {
        if (toTargetDist < attackRange)
        {
            ChangeState(BossState.ATTACK);
        }
    }


    void UpdateAttack_Delay()
    {
        if(toTargetDist < attackRange)
        {
            ChangeState(BossState.ATTACK);
        }
        else if(toTargetDist > attackRange)
        {
            ChangeState(BossState.MOVE);
        }
    }

    void AttackPattern_1() // 패턴 1 : 3가지 방법으로 공격
    {
        StartCoroutine(C_AttackPatten_1());
    }

    void ChangeAttackPos(Vector3 attackPos)
    {

    }

    Vector3 dir;
    float dist;
    float moveDist;
    IEnumerator C_AttackPatten_1()
    {
        print("코루틴 스타트");
        // AttackGround
        myAnim.SetTrigger("AttackGround");
        attackPos = target.transform.position;
        dir = (attackPos - transform.position).normalized;
        dist = (attackPos - transform.position).magnitude - attackRange;
        transform.forward = dir;
        if (dist > 0)
        {
            while (dist > moveDist)
            {
                moveDist += dist * Time.deltaTime;
                transform.Translate(dir * dist * Time.deltaTime / 2);
                yield return null;
            }
        }
        yield return new WaitForSeconds(3.233f);
        // AttackDash
        myAnim.SetTrigger("AttackDash");
        attackPos = target.transform.position;
        dir = (attackPos - transform.position).normalized;
        dist = (attackPos - transform.position).magnitude - attackRange;
        transform.forward = dir;
        if (dist > 0)
        {
            while (dist > moveDist)
            {
                moveDist += dist * Time.deltaTime;
                transform.Translate(dir * dist * Time.deltaTime / 2);
                yield return null;
            }
        }
        yield return new WaitForSeconds(4.067f);
        // AttackJump2
        myAnim.SetTrigger("AttackJump2");
        attackPos = target.transform.position;
        dir = (attackPos - transform.position).normalized;
        dist = (attackPos - transform.position).magnitude - attackRange;
        transform.forward = dir;
        if(dist > 0)
        {
            while (dist > moveDist)
            {
                moveDist += dist * Time.deltaTime;
                transform.Translate(dir * dist * Time.deltaTime/2);
                yield return null;
            }
        }
        yield return new WaitForSeconds(4.967f);
        ChangeState(BossState.MOVE);
        yield return null;
    }


    void AttackPattern_2() // 패턴 2 : 먼거리까지 닿는 칼 찍기 공격. 제자리 공격. 
    {
        // AttackJump1 애니메이션으로
        myAnim.SetTrigger("AttackJump1");
        attackPos = target.transform.position;
        dir = (attackPos - transform.position).normalized;
        transform.forward = dir;
        currTime = 0;
        while(currTime < 5)
        {
            currTime += Time.deltaTime;
        }
        ChangeState(BossState.MOVE);
    }

    int myAttackPattern = 0;
    int totalPatternCnt = 2;
    void OnDecideAttackPattern()
    {
        // 1번부터 번갈아 가면서 시행
        myAttackPattern++;
        myAttackPattern %= totalPatternCnt; // 패턴 개수로 나머지 구하기.
        if(myAttackPattern == 1)
        {
            AttackPattern_1();
        }
        if(myAttackPattern == 0)
        {
            AttackPattern_2();
        }
    }


    void OnDie()
    {

    }




    void MyRotate()
    {
        Vector2 forward = new Vector2(transform.position.z, transform.position.x);
        Vector2 steeringTarget = new Vector2(agent.steeringTarget.z, agent.steeringTarget.x);

        // 방향을 구한 뒤, 역함수로 각을 구함.
        Vector2 dir = steeringTarget - forward;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // 방향적용
        transform.eulerAngles = Vector3.up * angle;

        if (agent.enabled && target != null)
            agent.destination = target.transform.position;
    }

    float currTime;
    bool IsDelayComplete(float delayTime) // 딜레이 시간
    {
        // 시간을 증가시킨다.
        currTime += Time.deltaTime;
        // 만약에 시간이 delayTime보다 커지면
        if(currTime >= delayTime)
        {
            // 현재시간 초기화
            currTime = 0;
            // true 반환
            return true;
        }
        return false;
    }



}
