using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Y_AllyFSM : MonoBehaviour
{
    enum AllyState
    {
        //Idle,
        Move,
        Damaged,
        Die,
        ReturnToPlayer,
        Reborn
    }

    AllyState a_State;

    // Distance
    //public float findDistance = 10f;
    public float returnDistance = 30f;

    // Move
    public Vector3 moveDir = new Vector3(0, 0, 0);
    public float currTime = 5;
    public float moveTime = 5;

    // Return
    Vector3 playerPos;

    // Damaged
    public bool hasDamaged;


    #region 가져오기
    CharacterController cc;
    GameObject enemy;
    Y_HPSystem hp;
    Y_PlayerAttack pa;
    GameObject player;
    GameObject allyBody;
    float enemyAttackPower;
    Animator anim;

    #endregion

    public LayerMask enemyLayer;
    public Collider[] targets;

    void Start()
    {

        enemy = GameObject.Find("Enemy");
        if(enemy != null)
        enemyAttackPower = enemy.GetComponent<EnemyMove>().attackPower;
        player = GameObject.Find("Player");
        allyBody = GameObject.Find("AllyBody");

        cc = GetComponent<CharacterController>();
        hp = GetComponent<Y_HPSystem>();
        pa = GetComponent<Y_PlayerAttack>();

        anim = GetComponentInChildren<Animator>();

        hasDamaged = false;
        
        moveDir = new Vector3(1, 0, 0);

        a_State = AllyState.Move;



    }


    void Update()
    {
        switch(a_State)
        {
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

        anim.SetFloat("MOVE", moveDir.magnitude);
    }

    public int quadrant1 = 0;
    public int quadrant2 = 0;
    public int quadrant3 = 0;
    public int quadrant4 = 0;

    float forwardDot = 0;
    float rightDot = 0;

    void Move()
    {
        // 퀘스트가 있으면 몬스터를 피하면서 퀘스트 목표 장소로 이동

        // 퀘스트가 없으면 몬스터를 피해 이동
        // 8방 or 각도 나누기 + normalized 잊지 말 것

        // 일단은 3초마다 한번씩 랜덤한 방향으로 움직이게 해놓고 기본공격/스킬 하나 구현한 이후에 생각해보자

        currTime += Time.deltaTime;
        if (currTime > 3)
        {
            if (hp.isDead == false)
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

            }
            
            currTime = 0;

        }
        /////////////////////////////////////////////////////////////////////
        //currTime += Time.deltaTime;
        //if(currTime > 5)
        //{
        //    print("MoveStart");
        //    targets = Physics.OverlapSphere(transform.position, 100000f, enemyLayer);
        //    quadrant1 = 0;
        //    quadrant2 = 0;
        //    quadrant3 = 0;
        //    quadrant4 = 0;
        //    foreach (Collider target in targets)
        //    {
        //        forwardDot = Vector3.Dot(Vector3.forward, target.transform.position);
        //        rightDot = Vector3.Dot(Vector3.right, target.transform.position);

        //        if(forwardDot > 0 && rightDot > 0)
        //        {
        //            quadrant1++;
        //        }
        //        else if(forwardDot > 0 && rightDot < 0)
        //        {
        //            quadrant2++;
        //        }
        //        else if (forwardDot < 0 && rightDot < 0)
        //        {
        //            quadrant3++;
        //        }
        //        else if (forwardDot < 0 && rightDot > 0)
        //        {
        //            quadrant4++;
        //        }
        //    }

        //    int[] quadrants = new int[4]{quadrant1, quadrant2, quadrant3, quadrant4};


        //    // a, b 양수인지 음수인지에 따라 방향 결정
        //    // 방향은 사분면 고르고 그 다음에 가장 가까운 적에게
        //    // 5초마다 한 번씩 방향 바꾼다


        //    if(Mathf.Max(quadrants) == quadrant1)
        //    {
        //        moveDir = transform.position + new Vector3(1, 0, 1).normalized;
        //    }
        //    else if(Mathf.Max(quadrants) == quadrant2)
        //    {
        //        moveDir = transform.position + new Vector3(-1, 0, 1).normalized;
        //    }
        //    else if (Mathf.Max(quadrants) == quadrant3)
        //    {
        //        moveDir = transform.position + new Vector3(-1, 0, -1).normalized;
        //    }
        //    else if (Mathf.Max(quadrants) == quadrant4)
        //    {
        //        moveDir = transform.position + new Vector3(1, 0, -1).normalized;
        //    }

        //    currTime = 0;
        //}


        ////////////////////////////////////////////////////////////////




        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            a_State = AllyState.ReturnToPlayer;
        }

    }

    void Damaged()
    {
        // 화면 가장자리 빨간색으로 되는 함수
        //print("Damaged"); // 일단은 프린트
        a_State = AllyState.Move;


    }

    void Die()
    {
        // Die 애니메이션 실행
        print("Die"); // 일단은 프린트
        pa.RemoveFeather();
        pa.curBAttTime = 0;
        pa.curEAttTime = 0;
        pa.curRAttTime = 0;
        a_State = AllyState.Reborn;

    }

    void ReturnToPlayer()
    {
        if (!pa.isBAttack && !pa.isESkill && !pa.isRSkill)
        {
            // 일단은 플레이어 오른쪽 옆으로 순간이동하자
            transform.position = player.transform.position + new Vector3(2, 0, 0);
            a_State = AllyState.Move;
        }

    }

    void Reborn()
    {
        hp.Reborn();
        a_State = AllyState.Move;
    }

    public void HitAlly(float hitPower)
    {
        //hp.Damaged(hitPower);

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

    public void playChooseDir()
    {
        StartCoroutine(ChooseDir());
    }
}
