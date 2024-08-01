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
        DEAD
    }
    FindPlayers findPlayers;
    NokturneState currState;
    public GameObject attackIndicatorPos;
    public GameObject target;
    NavMeshAgent agent;
    Vector3 rayDir;
    Vector3 attackDir;
    public float attackDelay;
    WaitForSeconds attackDelays;
    Ray toTarget;
    RaycastHit hitInfo;
    bool canAttack;
    bool attacking;
    public float attackSpeed;
    Vector3 destination;
    Vector3 indicatorOrgPos;
    Coroutine attackCoroutine;
    float attackRange = 10;
    float moveDist;
    float toTargetDist;

    private void Awake()
    {
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
        canAttack = Physics.Raycast(toTarget, out hitInfo, attackRange, 1 << LayerMask.NameToLayer("Player"));
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
            case NokturneState.DEAD:
                break;
            default:
                break;
        }


    }


    public void ChangeState(NokturneState state)
    {
        print(currState + "------>" + state);


        currState = state;


        switch (currState)
        {
            case NokturneState.IDLE:
                break;
            case NokturneState.MOVE:
                OnMove();
                break;
            case NokturneState.ATTACK:
                OnAttack();
                break;
            case NokturneState.DEAD:
                break;
            default:
                break;
        }
    }

    void UpdateIdle()
    {
        ChangeState(NokturneState.MOVE);
    }
    void UpdateMove()
    {
        if(canAttack)
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
        print((agent.destination - transform.position).magnitude);
        if(Mathf.Approximately((agent.destination-transform.position).magnitude, 0.5f))
        {
            if (toTargetDist < attackRange)
            {
                ChangeState(NokturneState.ATTACK);
            }
            else
            {
                ChangeState(NokturneState.MOVE);
            }
        }
    }

    public void OnMove()
    {
        agent.enabled = true;
        agent.destination = target.transform.position;
    }


    IEnumerator NocturneAttack()
    {
        // 타겟의 위치를 저장하고
        // 방향을 타겟쪽으로 정하고
        attackDir = target.transform.position - transform.position;
        // 이동을 멈추자 (agent를 끄고)
        agent.enabled = false;
        attackIndicatorPos.SetActive(true);
        attackIndicatorPos.transform.SetParent(null);
        attackIndicatorPos.transform.forward = new Vector3(attackDir.x, 0, attackDir.z);
        // attackDelays만큼 대기했다가
        yield return attackDelays;
        while (moveDist < attackRange)
        {
            moveDist += attackSpeed * Time.deltaTime;
            // 방향쪽으로 정해진 거리만큼 움직이자.
            transform.Translate(attackDir.normalized * attackSpeed * Time.deltaTime);
            // 벽이 나오면 움직임을 멈추자.
            if (Physics.Raycast(transform.position, transform.forward, 2))
            {
                attackDir = Vector3.zero;
            }
            yield return null;
        }
        attackIndicatorPos.SetActive(false);
        attackIndicatorPos.transform.SetParent(transform);
        attackIndicatorPos.transform.localPosition = indicatorOrgPos;
    }
}
