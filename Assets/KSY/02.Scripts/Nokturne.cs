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
    Vector3 dir;
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
            dir = target.transform.position - transform.position;
        }
        toTarget = new Ray(transform.position, dir);
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
                OnAttack();
            }
            else
            {
                ChangeState(NokturneState.MOVE);
            }
        }
    }

    public void OnMove()
    {
        agent.destination = target.transform.position;
    }


    IEnumerator NocturneAttack()
    {
        
        agent.speed = 0;
        print("코루틴 부름");
        agent.destination = transform.position + dir.normalized * attackRange;
        if(Physics.Raycast(transform.position, dir.normalized, out RaycastHit info, attackRange, (-1) - (1 << LayerMask.NameToLayer("Player"))))
        {
            agent.destination = info.point - dir.normalized;
        }
        print("코루틴에서" + agent.destination);
        attackDir = agent.destination - transform.position;
        attackIndicatorPos.SetActive(true);
        attackIndicatorPos.transform.SetParent(null);
        attackIndicatorPos.transform.forward = new Vector3(attackDir.x, 0, attackDir.z);
        yield return attackDelays;
        agent.speed = attackSpeed;
        attackIndicatorPos.SetActive(false);
        attackIndicatorPos.transform.SetParent(transform);
        attackIndicatorPos.transform.localPosition = indicatorOrgPos;

    }
}
