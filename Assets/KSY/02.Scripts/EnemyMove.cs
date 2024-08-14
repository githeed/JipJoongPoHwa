using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class EnemyMove : MonoBehaviour
{

    [Header("조절 가능")]
    [Tooltip("공격력")]
    public float attackPower;
    [Tooltip("죽을 때 플레이어에게 주는 경험치 양")]
    public float giveEXP;
    [Tooltip("소환될 때 플레이어로 부터 최소 거리")]
    public float distanceMin;
    [Tooltip("소환될 때 플레이어로 부터 최대 거리")]
    public float distanceMax;

    [Header("터치 금지")]

    public GameObject target;

    public IObjectPool<GameObject> pool { get; set; }
    

    float dist0;
    float dist1;
    FindPlayers findPlayers;
    EnemyHp enemyHp;
    bool canAttack;
    protected Coroutine attackCoroutine;
    public NavMeshAgent agent;

    protected H_PlayerAttack playerCsH;
    protected Y_PlayerAttack playerCsY;
    protected Vector2 distance;
    float rand;
    float randDirX;
    float randDirZ;
    Vector3 dir;

    public string[] debug = new string[10];

    public GameObject damageUIPos;


    private void Awake()
    {
        findPlayers = GetComponent<FindPlayers>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        enemyHp = GetComponent<EnemyHp>();
        enemyHp.onDie = onDie;
        enemyHp.onDamageUI = OnDamageUI;
    }


    private void Update()
    {
        if (GameManager.instance.isStop) return;
        target = findPlayers.target;
        Vector2 forward = new Vector2(transform.position.z, transform.position.x);
        Vector2 steeringTarget = new Vector2(agent.steeringTarget.z, agent.steeringTarget.x);

        // 방향을 구한 뒤, 역함수로 각을 구함.
        Vector2 dir = steeringTarget - forward;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // 방향적용
        transform.eulerAngles = Vector3.up * angle;

        if(agent.enabled && target != null)
        agent.destination = target.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            canAttack = true;
            if (target != null)
            {
                playerCsH = other.GetComponent<H_PlayerAttack>();
                playerCsY = other.GetComponent<Y_PlayerAttack>();
            }
            attackCoroutine = StartCoroutine(Attack()); // Coroutine형으로 받아서 참조하여 관리.
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            StopCoroutine(attackCoroutine); // 코루틴을 하나만 할 수 있게.
            canAttack = false;
            playerCsY = null;
            playerCsH = null;
        }
    }

    void OnDamageUI(float dmg)
    {
        GameObject damageUI = ObjectPoolManager.instance.damageUIPool.Get();
        EnemyDamageUI damageUISc = damageUI.GetComponent<EnemyDamageUI>();
        damageUISc.UpdateAmount(dmg);
        damageUI.transform.position = damageUIPos.transform.position;
    }


    void onDie()
    {
        agent.enabled = false;
        pool.Release(this.gameObject);
        H_PlayerManager.instance.SpawnExp(transform.position);
    }

    public void OnNav()
    {
        StartCoroutine(Co_OnNav());
    }

    IEnumerator Co_OnNav()
    {
        yield return null;
        agent.enabled = true;
    }
    IEnumerator Attack()
    {

        while (canAttack)
        {
            if(playerCsH != null)
            {
                playerCsH.UpdateHp(attackPower);
            }
            if(playerCsY != null)
            {
                playerCsY.UpdateHp(attackPower);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

}
