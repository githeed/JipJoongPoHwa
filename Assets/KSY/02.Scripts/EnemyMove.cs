﻿using NUnit.Framework;
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
    GameObject[] targets;
    GameObject target;
    List<float> distanceToTargets = new List<float>();
    H_PlayerAttack playerCsH;
    Y_PlayerAttack playerCsY;
    public float attackPower;

    bool canAttack;
    Coroutine attackCoroutine;
    NavMeshAgent agent;

    public float maxHp;
    float curHp;

    public float distanceMin;
    public float distanceMax;
    Vector2 distance;
    float rand;
    float randDirX;
    float randDirZ;
    Vector3 dir;

    public string[] debug = new string[10];


    private void Awake()
    {
        targets = GameObject.FindGameObjectsWithTag("Player");
        for(int i = 0; i < targets.Length; i++)
        {
            distanceToTargets.Add((targets[i].transform.position - transform.position).magnitude);
        }
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
    }

    private void OnEnable()
    {
        curHp = maxHp;
    }

    void Update()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            distanceToTargets[i] = (targets[i].transform.position - transform.position).magnitude;
        }
        for (int i = 0; i < targets.Length; i++)
        {
            if (distanceToTargets[i] == distanceToTargets.Min())
            {
                target = targets[i];
            }
        }



        if (targets.Length >= 1)
        debug[0] = (targets[0].name);
        if (targets.Length >=2)
        debug[1] = (targets[1].name);
        debug[2] = ( target.name);
        Vector2 forward = new Vector2(transform.position.z, transform.position.x);
        Vector2 steeringTarget = new Vector2(agent.steeringTarget.z, agent.steeringTarget.x);

        // 방향을 구한 뒤, 역함수로 각을 구함.
        Vector2 dir = steeringTarget - forward;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // 방향적용
        transform.eulerAngles = Vector3.up * angle;

        if(agent.enabled)
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

    /// <summary>
    /// 매개변수에 float형으로 공격력(attackPower)넣어주면 됨.
    /// </summary>
    /// <param name="dmg"></param>
    public void UpdateHp(float dmg)
    {
        curHp -= dmg;
        if(curHp <= 0)
        {
            print(curHp);
            agent.enabled = false;
            pool.Release(this.gameObject);
        }
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
