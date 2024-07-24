﻿using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    // Distance
    public float findDistance = 10f;
    public float attackDistance = 10f;
    public float returnDistance = 20f;

    // Move
    public float moveSpeed = 1f;
    Vector3 moveDir = new Vector3(0, 0, 0);

    // Return
    Vector3 playerPos;

    CharacterController cc;

    #region 가져오기
    Y_EnemyAttack enemy;
    Y_HPSystem hp;
    GameObject player;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        a_State = AllyState.Move;

        enemy = GameObject.Find("Enemy").GetComponent<Y_EnemyAttack>();
        player = GameObject.Find("Player_Y_copied");

        cc = GetComponent<CharacterController>();
        hp = GetComponent<Y_HPSystem>();

        StartCoroutine(ChooseDir());

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
        if(Vector3.Distance(transform.position, enemy.transform.position) < findDistance)
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
        cc.Move(moveDir * moveSpeed * Time.deltaTime);

        // Player 에게 이동
        if(Vector3.Distance(transform.position, player.transform.position) > returnDistance)
        {
            a_State = AllyState.ReturnToPlayer;
        }

    }

    void Damaged()
    {
        // 화면 가장자리 빨간색으로 되는 함수
        print("Damaged"); // 일단은 프린트
        a_State = AllyState.Move;


    }

    void Die()
    {
        // Die 애니메이션 실행
        print("Die"); // 일단은 프린트
        a_State = AllyState.Reborn;

    }

    void ReturnToPlayer()
    {

        // 일단은 플레이어 오른쪽 옆으로 순간이동하자
        transform.position = player.transform.position + new Vector3(2, 0, 0);
        a_State = AllyState.Move;

    }

    void Reborn()
    {
        hp.Reborn();
        a_State = AllyState.Move;
    }

    public void HitPlayer(float hitPower)
    {
        hp.Damaged(enemy.attackPower);
        print("HitPlayer");
        if(hp.currHealth > 0)
        {
            a_State = AllyState.Damaged;
        }
        else
        {
            a_State = AllyState.Die;
        }
    }

    private IEnumerator ChooseDir()
    {
        while (hp.isDead == false)
        {
            int randInt = Random.Range(1, 9);

            switch (randInt)
            {
                case 1:
                    moveDir = new Vector3(1, 0, 0);
                    break;
                case 2:
                    moveDir = new Vector3(1, 0, 1).normalized;
                    break;
                case 3:
                    moveDir = new Vector3(0, 0, 1);
                    break;
                case 4:
                    moveDir = new Vector3(-1, 0, 1).normalized;
                    break;
                case 5:
                    moveDir = new Vector3(-1, 0, 0);
                    break;
                case 6:
                    moveDir = new Vector3(-1, 0, -1).normalized;
                    break;
                case 7:
                    moveDir = new Vector3(0, 0, -1);
                    break;
                case 8:
                    moveDir = new Vector3(1, 0, -1).normalized;
                    break;

            }
            yield return new WaitForSecondsRealtime(3f);
        }
        
   
    }    
}
