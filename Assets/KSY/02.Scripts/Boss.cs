using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    public enum BossState
    {
        IDLE,
        MOVE,
        ATTACK,
        ATTACK_DELAY,
        DEAD
    }
    public GameObject target;
    EnemyHp enemyHp;
    FindPlayers findPlayers;
    NavMeshAgent agent;
    public BossState currState;
    Animator myAnim;

    public float attackRange;
    public float moveSpeed;

    void Start()
    {
        enemyHp = GetComponent<EnemyHp>();
        enemyHp.onDie = OnDie;
        findPlayers = GetComponent<FindPlayers>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        myAnim = GetComponent<Animator>();
    }

    void Update()
    {
        
        target = findPlayers.target;
        switch (currState)
        {
            case BossState.IDLE:
                break;
            case BossState.MOVE:
                break;
            case BossState.ATTACK:
                break;
            case BossState.ATTACK_DELAY:
                break;
            case BossState.DEAD:
                break;
            default:
                break;
        }

    }

    void ChangeState(BossState state)
    {

        currState = state;

        switch (currState)
        {
            case BossState.IDLE:
                break;
            case BossState.MOVE:
                break;
            case BossState.ATTACK:
                break;
            case BossState.ATTACK_DELAY:
                break;
            case BossState.DEAD:
                break;
            default:
                break;
        }
    }

    void OnDie()
    {

    }
}
