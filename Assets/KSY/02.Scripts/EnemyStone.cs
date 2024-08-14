using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStone : MonoBehaviour
{
    [Header("조절 가능")]
    [Tooltip("피격시 빨개지는 이펙트 지속 시간")]
    public float effTime;
    [Tooltip("스톤의 공격력")]
    public float attackPower;
    [Tooltip("각 공격들의 범위")]
    public float attackRange;
    [Tooltip("각 공격들이 표시된 후 데미지가 들어갈 때 까지의 시간")]
    public float attackCoolTime;
    [Tooltip("스톤의 공격 범위")]
    public float myMaxRange;
    [Tooltip("공격 소환 쿨타임")]
    public float indiSpawnCoolTime;
    [Tooltip("나타났을 때 플레이어 땡겨오는 속도")]
    public float drawSpeed;



    [Header("터치 금지")]
    public List<GameObject> indiList = new List<GameObject>();
    public Boss boss;
    
    GameObject player0;
    GameObject player1;
    float distToPlayer0;
    float distToPlayer1;

    public GameObject rangeCanvasGo;
    public GameObject indicatorPrf;
    public GameObject indicator;
    IndicatorPrefOfStone indicatorCs;
    int minIndiCnt = 10;
    float currTime;
    EnemyHp myHp;
    WaitForSeconds effTimeSec;
    Material myMaterial;
    Color orgColor;

    public bool isStart;

    void Start()
    {
        myHp = GetComponent<EnemyHp>();
        myHp.onDie = OnDie;
        myHp.damageEff = DamageEff;
        myMaterial = GetComponentInChildren<MeshRenderer>().material;
        effTimeSec = new WaitForSeconds(effTime);
        orgColor = myMaterial.color;
        rangeCanvasGo.transform.localScale = Vector3.one * myMaxRange * 2.5f;
        for(int i = 0; i < minIndiCnt; i++)
        {
            indicator = Instantiate(indicatorPrf);
            indiList.Add(indicator);
            indicator.SetActive(false);
            indicatorCs = indicator.GetComponent<IndicatorPrefOfStone>();
            indicatorCs.attackPower = attackPower;
            indicatorCs.attackCoolTime = attackCoolTime;
            indicatorCs.attackRange = attackRange;
            indicatorCs.enemyStone = this;
        }
        player0 = GameObject.FindWithTag("Player");
        player1 = GameObject.FindWithTag("Player1");
        isStart = true;
    }

    void Update()
    {
        distToPlayer0 = Vector3.Distance(transform.position, player0.transform.position);
        distToPlayer1 = Vector3.Distance(transform.position, player1.transform.position);

        IsStart();


        if (isStart) return;

        currTime += Time.deltaTime;
        if(currTime > indiSpawnCoolTime)
        {
            currTime = 0;
            Vector2 rand = Random.insideUnitCircle * myMaxRange;
            if(indiList.Count == 0)
            {
                indicator = Instantiate(indicatorPrf);
                indicatorCs = indicator.GetComponent<IndicatorPrefOfStone>();
                indicatorCs.attackPower = attackPower;
                indicatorCs.attackCoolTime = attackCoolTime;
                indicatorCs.attackRange = attackRange;
                indicatorCs.enemyStone = this;
            }
            else
            {
                indicator = indiList[0];
                indiList.RemoveAt(0);
            }
            
            if(!indicator.activeSelf) indicator.SetActive(true);
            indicator.transform.position = transform.position + Vector3.up + Vector3.right * rand.x + Vector3.forward * rand.y;
        }

        
        if(distToPlayer0 > myMaxRange)
        {
            player0.transform.position = transform.position + (player0.transform.position - transform.position).normalized * myMaxRange;
        }
        if(distToPlayer1 > myMaxRange)
        {
            player1.transform.position = transform.position + (player1.transform.position - transform.position).normalized * myMaxRange;
        }

    }

    void IsStart()
    {
        if (distToPlayer0 < myMaxRange)
        {
            isStart = false;
            return;
        }
        if (distToPlayer0 > myMaxRange)
        {
            player0.transform.Translate((transform.position - player0.transform.position).normalized * drawSpeed * Time.deltaTime, Space.World);
            player1.transform.position = transform.position + (player1.transform.position - transform.position).normalized * (myMaxRange-2);
        }
        
    }

    void DamageEff()
    {
        StartCoroutine(C_DamageEff());
    }

    void OnDie()
    {
        // 사라지고 Boss 다시 활동
        transform.GetChild(0).gameObject.SetActive(false);
        boss.ChangeState(Boss.BossState.MOVE);
        boss.transform.position = transform.position + Vector3.up;
        gameObject.SetActive(false);
    }
    
    IEnumerator C_DamageEff()
    {
        myMaterial.color = Color.red;
        yield return effTimeSec;
        myMaterial.color = orgColor;
    }
}
