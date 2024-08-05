﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Nokturne : MonoBehaviour
{
    public enum NokturneState
    {
        IDLE,
        MOVE,
        ATTACK,
        ATTACK_DELAY,
        DEAD
    }

    EnemyHp enemyHp;
    FindPlayers findPlayers;
    NokturneState currState;

    public GameObject attackIndicatorPos;
    public GameObject target;
    NavMeshAgent agent;

    Vector3 rayDir;
    Vector3 attackDir;
    Vector3 targetPos;

    public float attackDelay;
    WaitForSeconds attackDelays;

    Ray toTarget;
    RaycastHit hitInfo;

    bool canAttack;
    bool attacking;
    public float attackSpeed;
    public float attackPower;

    Vector3 destination;
    Vector3 indicatorOrgPos;
    Coroutine attackCoroutine;

    float attackRange = 10;
    float moveDist;
    float toTargetDist;

    H_PlayerAttack playerCsH;
    Y_PlayerAttack playerCsY;

    private void Awake()
    {
        enemyHp = GetComponent<EnemyHp>();
        enemyHp.onDie = OnDie;
        currState = NokturneState.IDLE;
        attackIndicatorPos.SetActive(false);
        findPlayers = GetComponent<FindPlayers>();
        attackDelays = new WaitForSeconds(attackDelay);
        agent = GetComponent<NavMeshAgent>();
        indicatorOrgPos = attackIndicatorPos.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        target = findPlayers.target;
        toTargetDist = (target.transform.position - transform.position).magnitude;
        if (target != null)
        {
            rayDir = target.transform.position - transform.position;
        }
        toTarget = new Ray(transform.position, rayDir);
        if(Physics.Raycast(toTarget, out hitInfo, attackRange))
        {
            if(hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                canAttack = true;
            }
            else
            {
                canAttack = false;
            }
        }
        switch (currState)
        {
            case NokturneState.IDLE:
                UpdateIdle();
                break;
            case NokturneState.MOVE:
                UpdateMove();
                break;
            case NokturneState.ATTACK:
                UpdateAttack();
                break;
            case NokturneState.ATTACK_DELAY:
                break;
            case NokturneState.DEAD:
                break;
            default:
                break;
        }


    }


    public void ChangeState(NokturneState state)
    {
        currState = state;
        agent.enabled = true;

        switch (currState)
        {
            case NokturneState.IDLE:
                break;
            case NokturneState.MOVE:
                break;
            case NokturneState.ATTACK:
                OnAttack();
                break;
            case NokturneState.ATTACK_DELAY:
                OnATTACK_DELAY();
                break;
            case NokturneState.DEAD:
                break;
            default:
                break;
        }
    }

    private void OnATTACK_DELAY()
    {
        if (toTargetDist < attackRange && canAttack)
        {
            ChangeState(NokturneState.ATTACK);
        }
        else
        {
            ChangeState(NokturneState.MOVE);
        }
    }

    void UpdateIdle()
    {
        ChangeState(NokturneState.MOVE);
    }
    void UpdateMove()
    {
        agent.destination = target.transform.position;
        if (toTargetDist < attackRange && canAttack)
        {
            ChangeState(NokturneState.ATTACK);
        }
    }
    void OnAttack()
    {
        StartCoroutine(NocturneAttack());
    }

    void UpdateAttack()
    {
        //print("공격중 목표와의 거리 : " + (targetPos - transform.position).magnitude);
    }


    public void OnDie()
    {
        Destroy(attackIndicatorPos);
        Destroy(gameObject);
    }


    IEnumerator NocturneAttack()
    {
        // 타겟의 위치를 저장하고
        targetPos = target.transform.position;
        // 방향을 타겟쪽으로 정하고
        attackDir = targetPos - transform.position;
        attackDir -= Vector3.up * attackDir.y; // y값 없애기
        transform.forward = attackDir.normalized;
        // 이동을 멈추자 (agent를 끄고)
        agent.enabled = false;
        attackIndicatorPos.SetActive(true);
        attackIndicatorPos.transform.forward = new Vector3(attackDir.x, 0, attackDir.z);
        attackIndicatorPos.transform.SetParent(null);
        // attackDelays만큼 대기했다가
        yield return attackDelays;
        // 돌진
        while (moveDist < attackRange)
        {
            moveDist += attackSpeed * Time.deltaTime;
            // 방향쪽으로 정해진 거리만큼 움직이자.
            transform.Translate(attackDir.normalized * attackSpeed * Time.deltaTime, Space.World);
            // 벽이 나오면 움직임을 멈추자.
            if (Physics.Raycast(transform.position + Vector3.up*2, transform.forward, agent.radius, 1<<LayerMask.NameToLayer("Walls")))
            {
                attackDir = Vector3.zero;
            }
            yield return null;
        }
        moveDist = 0;
        attackIndicatorPos.SetActive(false);
        attackIndicatorPos.transform.SetParent(transform);
        attackIndicatorPos.transform.localPosition = indicatorOrgPos;
        ChangeState(NokturneState.ATTACK_DELAY);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerCsH = other.GetComponent<H_PlayerAttack>();
            playerCsY = other.GetComponent<Y_PlayerAttack>();
            if (playerCsH != null)
            {
                playerCsH.UpdateHp(attackPower);
                playerCsH = null;
            }
            if (playerCsY != null)
            {
                playerCsY.UpdateHp(attackPower);
                playerCsY = null;
            }
        }
    }
}
