using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyFSM_Y : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        a_State = AllyState.Idle;
        enemy = GameObject.Find("Enemy").transform;
        
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
