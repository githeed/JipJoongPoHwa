using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_AllyFSM : MonoBehaviour
{
    enum AllyState
    {
        Idle,
        Move,
        Damaged,
        Die,
        ReturnToPlayer,
        Reborn
    }

    AllyState a_State;
    public float findDistance = 20f;
    Transform enemy;

    public float attackDistance = 10f;
    public float moveSpeed = 5f;

    CharacterController cc;

    // Start is called before the first frame update
    void Start()
    {
        a_State = AllyState.Idle;
        enemy = GameObject.Find("Enemy").transform;

        cc = GetComponent<CharacterController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(a_State)
        {
            case AllyState.Idle:
                Idle();
                break;
            case AllyState.Move:
                Move();
                break;
            case AllyState.Damaged:
                Damaged();
                break;
            case AllyState.Die:
                Die();
                break;
            case AllyState.ReturnToPlayer:
                ReturnToPlayer();
                break;
            case AllyState.Reborn:
                Reborn();
                break;
        }
    }

    void Idle()
    {
        if(Vector3.Distance(transform.position, enemy.position) < findDistance)
        {
            a_State = AllyState.Move;
            print("상태 전환: Idle -> Move");
        }

    }

    void Move()
    {
        // 퀘스트가 있으면 몬스터를 피하면서 퀘스트 목표 장소로 이동

        // 퀘스트가 없으면 몬스터를 피해 이동
        // 8방 or 가 + normalized 잊지 말 것

        // 일단은 3초마다 한번씩 랜덤한 방향으로 움직이게 해놓고 기본공격/스킬 하나 구현한 이후에 생각해보자

    }

    void Damaged()
    {

    }

    void Die()
    {

    }

    void ReturnToPlayer()
    {

    }

    void Reborn()
    {

    }
}
