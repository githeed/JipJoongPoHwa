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
    public IObjectPool<GameObject> pool { get; set; }
    public GameObject target;
    float dist0;
    float dist1;
    FindPlayers findPlayers;
    EnemyHp enemyHp;

    protected H_PlayerAttack playerCsH;
    protected Y_PlayerAttack playerCsY;
    public float attackPower;
    public float giveEXP;

    bool canAttack;
    protected Coroutine attackCoroutine;
    public NavMeshAgent agent;

    public float distanceMin;
    public float distanceMax;
    protected Vector2 distance;
    float rand;
    float randDirX;
    float randDirZ;
    Vector3 dir;

    public string[] debug = new string[10];


    private void Awake()
    {
        findPlayers = GetComponent<FindPlayers>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        enemyHp = GetComponent<EnemyHp>();
        enemyHp.onDie = onDie;
    }


    private void Update()
    {
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
            attackCoroutine = StartCoroutine(Attack()); // Coroutine형으로 받아서 참조하여 관리.
            if (target != null)
            {
                playerCsH = other.GetComponent<H_PlayerAttack>();
                playerCsY = other.GetComponent<Y_PlayerAttack>();
            }
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


    public void UpdateHp(float dmg)
    {
        
    }

    void onDie()
    {
        agent.enabled = false;
        pool.Release(this.gameObject);
        H_PlayerManager.instance.UpdateExp(giveEXP);
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
            yield return new WaitForSeconds(1.0f);
        }
    }

}
