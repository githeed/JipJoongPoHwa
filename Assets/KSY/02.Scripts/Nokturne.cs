﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Nokturne : MonoBehaviour, IAnimatorInterface
{
    public enum NokturneState
    {
        IDLE,
        MOVE,
        ATTACK,
        ATTACK_DELAY,
        DEAD
    }

    [Header ("조절 가능")]
    [Tooltip("공격상태 후 다시 공격 가능할 때까지 걸리는 시간")]
    public float attackDelay;
    [Tooltip("공격할 때 돌진하는 속도")]
    public float attackSpeed;
    [Tooltip("공격력")]
    public float attackPower;
    [Tooltip("돌진 가능 거리")]
    public float attackRange = 20;
    public float motionDelay;

    [Header ("터치 금지")]
    public GameObject attackIndicatorPos;
    public GameObject target;
    NavMeshAgent agent;
    EnemyHp enemyHp;
    FindPlayers findPlayers;
    public NokturneState currState;

    Vector3 rayDir;
    Vector3 attackDir;
    Vector3 targetPos;

    WaitForSeconds motionDelays;

    Ray toTarget;
    RaycastHit hitInfo;

    bool canAttack;
    bool attacking;
    bool canReAttack;

    Vector3 destination;
    Vector3 indicatorOrgPos;
    Coroutine attackCoroutine;

    float moveDist;
    float toTargetDist;

    H_PlayerAttack playerCsH;
    Y_PlayerAttack playerCsY;

    Animator myAnim;

    float effTime = 0.1f;
    WaitForSeconds effTimeSec;
    Material myMaterial;
    Color orgColor;

    private void Awake()
    {
        enemyHp = GetComponent<EnemyHp>();
        enemyHp.onDie = OnDie;
        enemyHp.damageEff = DamageEff;
        myMaterial = GetComponentInChildren<SkinnedMeshRenderer>().material;
        effTimeSec = new WaitForSeconds(effTime);
        orgColor = myMaterial.color;

        currState = NokturneState.IDLE;
        attackIndicatorPos.SetActive(false);
        findPlayers = GetComponent<FindPlayers>();
        motionDelays = new WaitForSeconds(motionDelay);
        agent = GetComponent<NavMeshAgent>();
        indicatorOrgPos = attackIndicatorPos.transform.localPosition;
        myAnim = GetComponentInChildren<Animator>();
        canReAttack = true;
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
        toTarget = new Ray(transform.position + Vector3.up*2, rayDir);
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

    bool onAttack;

    public void ChangeState(NokturneState state)
    {
        currTime = 0;
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
        ChangeState(NokturneState.MOVE);
    }

    void UpdateIdle()
    {
        ChangeState(NokturneState.MOVE);
    }
    void UpdateMove()
    {
        currTime += Time.deltaTime;
        if(attackDelay < currTime)
        {
            canReAttack = true;
        }
        agent.destination = target.transform.position;
        if (toTargetDist < attackRange && canAttack && canReAttack)
        {
            currTime = 0;
            ChangeState(NokturneState.ATTACK);
        }
    }
    void OnAttack()
    {
        onAttack = true;
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
    }


    float currTime = 0;
    void UpdateAttack()
    {
        currTime += Time.deltaTime;
        if(currTime > motionDelay && onAttack)
        {
            myAnim.SetTrigger("Attack");
            canReAttack = false;
            onAttack = false;
            currTime = 0;
        }
        if (attacking)
        {
            AttackMove();
        }

    }

    void AttackMove()
    {
        // 돌진
        if (moveDist < attackRange)
        {
            moveDist += attackSpeed * Time.deltaTime;
            // 방향쪽으로 정해진 거리만큼 움직이자.
            transform.Translate(attackDir.normalized * attackSpeed * Time.deltaTime, Space.World);
            // 벽이 나오면 움직임을 멈추자.
            if (Physics.Raycast(transform.position + Vector3.up * 2, transform.forward, agent.radius, 1 << LayerMask.NameToLayer("Walls")))
            {
                attackDir = Vector3.zero;
            }
        }
        else
        {
            attacking = false;
            moveDist = 0;
        }
    }

    public void OnDie()
    {
        H_PlayerManager.instance.SpawnExp(transform.position);
        Destroy(attackIndicatorPos);
        Destroy(gameObject);
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
        // motionDelays만큼 대기했다가
        yield return motionDelays;
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

    public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Attack"))
        {
            attacking = true;
            
            
        }
    }

    public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Attack"))
        {
            attackIndicatorPos.SetActive(false);
            attackIndicatorPos.transform.SetParent(transform);
            attackIndicatorPos.transform.localPosition = indicatorOrgPos;
            attacking = false;
            ChangeState(NokturneState.MOVE);
        }
        
    }
}
