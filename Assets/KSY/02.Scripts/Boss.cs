using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Boss : MonoBehaviour, IAnimatorInterface
{
    public enum BossState
    {
        START,
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
    public GameObject stone;
    public EnemyStone enemyStone;
    public Transform stoneSpawnPos;

    public GameObject target;
    EnemyHp enemyHp;
    FindPlayers findPlayers;
    NavMeshAgent agent;

    Vector3 attackPos;

    public GameObject indicatorPref;
    GameObject indicator;
    IndicatorPref indicatorCS;

    GameObject mainCam;
    Transform mainCamOrgParent;
    public Transform mainCamTargetPos;
    public float camMoveSpeed = 40;
    Vector3 startDir;

    public BossState currState;
    Animator myAnim;
    float toTargetDist;
    Vector3 toTargetDir;

    IndicatorLongLine longIndicator;
    bool workingPattern01;

    public Transform bossMoveTarget;

    float effTime = 0.1f;
    WaitForSeconds effTimeSec;
    Material myMaterial;
    Color orgColor;

    public GameObject myBossHPUI;
    TextMeshProUGUI bossHPUItext;
    Image hPImage;

    public GameObject attackGroundEff;
    public GameObject attackGroundEff_1;
    public GameObject attackHorizontalEff;

    public bool cineStart = false;
    private void Awake()
    {
        enemyHp = GetComponent<EnemyHp>();
        agent = GetComponent<NavMeshAgent>();
        myAnim = GetComponent<Animator>();
        longIndicator = GetComponentInChildren<IndicatorLongLine>();
        mainCam = Camera.main.gameObject;
        mainCamOrgParent = mainCam.transform.parent;
        agent.enabled = false;
    }
    void Start()
    {
        cineStart = true;
        enemyHp.onDie = OnDie;
        enemyHp.damageEff = DamageEff;
        myMaterial = GetComponentInChildren<SkinnedMeshRenderer>().material;
        effTimeSec = new WaitForSeconds(effTime);
        orgColor = myMaterial.color;

        findPlayers = GetComponent<FindPlayers>();
        
        agent.speed = moveSpeed;
        
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
        ChangeState(BossState.START);

        if(myBossHPUI != null)
        {
            hPImage = myBossHPUI.transform.GetChild(1).GetComponent<Image>();
            bossHPUItext = myBossHPUI.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        hPImage.fillAmount = enemyHp.curHp / enemyHp.maxHp;
        bossHPUItext.text = $"{(int)enemyHp.curHp} / {(int)enemyHp.maxHp}";
        target = findPlayers.target;
        if (agent.enabled) MyRotate();
        //toTargetDir = target.transform.position - transform.position;
        toTargetDir = new Vector3(target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z);
        toTargetDist = toTargetDir.magnitude;
        print(mainCam.name);

        switch (currState)
        {
            case BossState.START:
                UpdateStart();
                break;
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

    

    public void ChangeState(BossState state)
    {
        print(currState + " ----> " + state);
        currState = state;
        myAnim.SetBool("MOVE", false);
        agent.enabled = false;
        currTime = 0;

        switch (currState)
        {
            case BossState.START:
                mainCam.transform.SetParent(null);
                break;
            case BossState.IDLE:
                OnIdle();
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
    void UpdateStart()
    {
        startDir = (bossMoveTarget.position - transform.position).normalized;
        if (Vector3.Distance(bossMoveTarget.position, transform.position)< 0.5f)
        {
            ChangeState(BossState.IDLE);
            return;
        }
        transform.forward = (startDir - Vector3.up * startDir.y).normalized;
        transform.Translate(startDir * moveSpeed*2 * Time.deltaTime, Space.World);
        if (Vector3.Distance(mainCam.transform.position, mainCamTargetPos.position) < 0.5f) 
        {
            mainCam.transform.position = mainCamTargetPos.position;
            return;
        }
        mainCam.transform.Translate((mainCamTargetPos.position - mainCam.transform.position).normalized * camMoveSpeed * Time.deltaTime, Space.World);
    }

    void OnIdle()
    {
        myAnim.SetTrigger("Roar");
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
    int totalPatternCnt = 2;
    void OnDecideAttackPattern()
    {
        //if(myAttackPattern == -1)
        //{
        //    myAttackPattern++;
        //    AttackPattern_3();
        //    return;
        //}
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
            //case 2:
            //    AttackPattern_3();
            //    break;
            default:
                break;
        }
    }

    void AttackPattern_3() // 패턴 3 : 기둥 패턴
    {
        stone.SetActive(true);
        if (stoneSpawnPos == null) stoneSpawnPos = GameObject.Find("StoneSpawnPos").transform;
        stone.transform.position = stoneSpawnPos.position;
        enemyStone = stone.GetComponent<EnemyStone>();
        enemyStone.boss = this;
        transform.position = Vector3.up * 1000;
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
            StartCoroutine(DelayAndStart(0.8f, () => attackGroundEff_1.SetActive(true))); // 이펙트 키기.
            StartCoroutine(DelayAndStart(2f, () => attackGroundEff_1.SetActive(false))); // 이펙트 끄기.
        }
        if (stateInfo.IsName("Attack_Dash"))
        {
            attackPos = target.transform.position;
            indicator.SetActive(true);
            indicator.transform.position = attackPos;
            indicatorCS.attackCoolTime = 1.5f; // 애니메이션에서 공격타이밍에 맞춤
            StartCoroutine(DelayAndStart(1.2f, () => attackHorizontalEff.SetActive(true))); // 이펙트 키기.
            StartCoroutine(DelayAndStart(2.5f, () => attackHorizontalEff.SetActive(false))); // 이펙트 끄기.
        }
        if (stateInfo.IsName("Attack_Jump02"))
        {
            attackPos = target.transform.position;
            indicator.SetActive(true);
            indicator.transform.position = attackPos;
            indicatorCS.attackCoolTime = 2.5f; // 애니메이션에서 공격타이밍에 맞춤
            StartCoroutine(DelayAndStart(2.2f, () => attackGroundEff_1.SetActive(true))); // 이펙트 키기.
            StartCoroutine(DelayAndStart(3.5f, () => attackGroundEff_1.SetActive(false))); // 이펙트 끄기.
        }

        if (stateInfo.IsName("Attack_Jump01"))
        {
            attackPos = target.transform.position;
            dir = new Vector3(attackPos.x - transform.position.x, 0, attackPos.z - transform.position.z).normalized;
            transform.forward = dir;
            
            longIndicator.gameObject.SetActive(true);

            StartCoroutine(DelayAndStart(3.17f, () => attackGroundEff.SetActive(false))); // 이펙트 끄기.
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

        if (stateInfo.IsName("Roar"))
        {
            AttackPattern_3();
            mainCam.transform.SetParent(mainCamOrgParent);
            mainCam.transform.localPosition = Vector3.zero;
            cineStart = false;

        }
    }

    IEnumerator DelayAndStart(float t, Action action)
    {
        print("됨");
        yield return new WaitForSeconds(t);
        action();
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
