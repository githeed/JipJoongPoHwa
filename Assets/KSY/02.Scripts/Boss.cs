using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Boss : MonoBehaviour, IAnimatorInterface
{
    public enum BossState
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
    [Tooltip("이동 속도")]
    public float moveSpeed;

    [Header("터치 금지")]
    public GameObject stonePrf;
    GameObject stone;
    public Transform stoneSpawnPos;

    public GameObject target;
    EnemyHp enemyHp;
    FindPlayers findPlayers;
    NavMeshAgent agent;

    Vector3 attackPos;

    public GameObject indicatorPref;
    GameObject indicator;
    IndicatorPref indicatorCS;

    public BossState currState;
    Animator myAnim;
    float toTargetDist;
    Vector3 toTargetDir;

    IndicatorLongLine longIndicator;
    bool workingPattern01;

    private void Awake()
    {
        enemyHp = GetComponent<EnemyHp>();
        agent = GetComponent<NavMeshAgent>();
        myAnim = GetComponent<Animator>();
        longIndicator = GetComponentInChildren<IndicatorLongLine>();
    }
    void Start()
    {
        
        enemyHp.onDie = OnDie;
        findPlayers = GetComponent<FindPlayers>();
        
        agent.speed = moveSpeed;
        
        ChangeState(BossState.MOVE);
        agent.updateRotation = false;
        
        longIndicator.attackPower = attackPower;
        longIndicator.attackDelay = 2.1f;
        if (stone == null)
        {
            stone = Instantiate(stonePrf);
            stone.SetActive(false);
        }
        if(indicator == null)
        {
            indicator = Instantiate(indicatorPref);
            indicatorCS = indicator.GetComponent<IndicatorPref>();
            indicatorCS.attackRange = attackRange;
            indicatorCS.attackPower = attackPower;
            indicator.SetActive(false);
        }
    }

    void Update()
    {
        target = findPlayers.target;
        if (agent.enabled) MyRotate();
        //toTargetDir = target.transform.position - transform.position;
        toTargetDir = new Vector3(target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z);
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


    Vector3 dir;
    float dist;
    float moveDist;
    IEnumerator C_AttackPatten_1()
    {
        workingPattern01 = true;
        // AttackGround
        // AttackDash
        // AttackJump2
        myAnim.SetTrigger("AttackGround");
        while (workingPattern01)
        {
            dir = new Vector3(attackPos.x - transform.position.x, 0, attackPos.z - transform.position.z).normalized;
            dist = new Vector3(attackPos.x - transform.position.x, 0, attackPos.z - transform.position.z).magnitude - 3;
            transform.forward = dir;
            moveDist = 0;
            if (dist > 0)
            {
                while (dist > moveDist)
                {
                    moveDist += dist * Time.deltaTime;
                    transform.Translate(dir * dist * Time.deltaTime / 2, Space.World);
                    yield return null;
                }
            }
            //transform.position = Vector3.MoveTowards(transform.position, attackPos, moveSpeed*Time.deltaTime);
            yield return null;
        }
        // 애니메이션이 끝나면 Move상태로 변경. -> 애니메이션의 OnStateExit에서 처리.
    }


    void AttackPattern_2() // 패턴 2 : 먼거리까지 닿는 칼 찍기 공격. 제자리 공격. 
    {
        // AttackJump1 애니메이션으로
        myAnim.SetTrigger("AttackJump1");

        // 애니메이션이 끝나면 Move상태로 변경. -> 애니메이션의 OnStateExit에서 처리.
    }

    int myAttackPattern = -1;
    int totalPatternCnt = 3;
    void OnDecideAttackPattern()
    {
        // 1번부터 번갈아 가면서 시행
        myAttackPattern++;
        myAttackPattern %= totalPatternCnt; // 패턴 개수로 나머지 구하기.
        switch (myAttackPattern)
        {
            case 0:
                AttackPattern_1();
                break;
            case 1:
                AttackPattern_2();
                break;
            case 2:
                AttackPattern_3();
                break;
            default:
                break;
        }
    }

    void AttackPattern_3() // 패턴 3 : 기둥 패턴
    {
        stone.SetActive(true);
        stone.transform.position = stoneSpawnPos.position;
    }

    void OnDie()
    {
        GameManager.instance.bISWin = true;
        SceneManager.LoadScene("EndUIScene");
    }


    public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Attack_Ground"))
        {
            attackPos = target.transform.position;
            indicator.SetActive(true);
            indicator.transform.position = attackPos;
            indicatorCS.attackCoolTime = 1f; // 애니메이션에서 공격타이밍에 맞춤
        }
        if (stateInfo.IsName("Attack_Dash"))
        {
            attackPos = target.transform.position;
            indicator.SetActive(true);
            indicator.transform.position = attackPos;
            indicatorCS.attackCoolTime = 1.5f; // 애니메이션에서 공격타이밍에 맞춤
        }
        if (stateInfo.IsName("Attack_Jump02"))
        {
            attackPos = target.transform.position;
            indicator.SetActive(true);
            indicator.transform.position = attackPos;
            indicatorCS.attackCoolTime = 2.5f; // 애니메이션에서 공격타이밍에 맞춤
        }

        if (stateInfo.IsName("Attack_Jump01"))
        {
            attackPos = target.transform.position;
            dir = new Vector3(attackPos.x - transform.position.x, 0, attackPos.z - transform.position.z).normalized;
            transform.forward = dir;
            
            longIndicator.gameObject.SetActive(true);
        }
    }

    public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Attack_Ground"))
        {
            myAnim.SetTrigger("AttackDash");
        }
        if (stateInfo.IsName("Attack_Dash"))
        {
            myAnim.SetTrigger("AttackJump2");
        }
        if (stateInfo.IsName("Attack_Jump02"))
        {
            workingPattern01 = false;
            ChangeState(BossState.MOVE);
        }

        if (stateInfo.IsName("Attack_Jump01"))
        {
            ChangeState(BossState.MOVE);
        }
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
