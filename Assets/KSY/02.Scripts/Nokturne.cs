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
    Coroutine attackCoroutine;
    float attackRange = 10;
    float moveDist;

    // Start is called before the first frame update
    void Start()
    {
        currState = NokturneState.IDLE;
        attackIndicatorPos.SetActive(false);
        findPlayers = GetComponent<FindPlayers>();
        attackDelays = new WaitForSeconds(attackDelay);
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        target = findPlayers.target;
        if (target != null)
        {
            dir = target.transform.position - transform.position;
        }
        toTarget = new Ray(transform.position, dir);
        canAttack = Physics.Raycast(toTarget, out hitInfo, attackRange, 1 << LayerMask.NameToLayer("Player"));
        print("업데이트에서" + agent.destination);
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
        if(Mathf.Approximately((agent.destination-transform.position).magnitude, 0.1f))
        {
            if (dir.magnitude < attackRange)
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
        print("코루틴 부름");
        agent.destination = transform.position + dir.normalized * attackRange;
        print("코루틴에서" + agent.destination);
        attackDir = agent.destination - transform.position;
        agent.speed = 0;
        attackIndicatorPos.SetActive(true);
        attackIndicatorPos.transform.forward = new Vector3(attackDir.x, 0, attackDir.z);
        yield return attackDelays;
        agent.speed = attackSpeed;
        
    }
}
