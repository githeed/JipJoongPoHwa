using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Y_NavMesh : MonoBehaviour
{
    public NavMeshAgent agent;
    GameObject player;
    GameObject ally;
    Y_AllyFSM allyFSM;
    Y_PlayerAttack yp;

    float defaultDist = 20f;
    public float moveSpeed = 10f;
    Y_PlayerAttack playerAttack;
    public GameObject boss;
    public GameObject bossStone;
    bool bossExist;
    Boss bossCs;
    float currTime;
    float changeTime;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        player = GameObject.Find("Player");
        ally = GameObject.Find("Ally");
        allyFSM = ally.GetComponent<Y_AllyFSM>();
        yp = ally.GetComponent<Y_PlayerAttack>();
        playerAttack = GetComponent<Y_PlayerAttack>();
    }
    void Start()
    {
        agent.speed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {

        currTime += Time.deltaTime;
        //float distToTarget = Vector3.Distance(player.transform.position, transform.position);

        //if(distToTarget > defaultDist)
        //{
        //    agent.destination = (player.transform.position);
        //}
        //else
        //{
        //if (GameManager.instance.canPick) bossExist = true;
        //if (bossExist) bossCs = GameManager.instance.bossCs;

        if (GameManager.instance.bossSpawn)
        {
            if(bossCs == null)
            {
                bossCs = GameObject.FindWithTag("Boss").GetComponent<Boss>();
                print("111111111");
            }
            else if(bossCs.stone.activeSelf)
            {
                agent.destination = bossCs.stone.transform.position - (playerAttack.featherDist - 5) * Vector3.forward;
                print("2222222222");
            }
            else if(!bossCs.stone.activeSelf)
            {
                agent.destination = bossCs.transform.position - (playerAttack.featherDist - 5) * Vector3.forward;
                print("33333333333");
            }
        }
        else if(!playerAttack.isESkill && !playerAttack.isBAttack && !playerAttack.isRSkill)
        {
            //if(currTime > changeTime)
            //{
                agent.destination = player.transform.position + 5 * allyFSM.moveDir; // (player.transform.position + allyFSM.moveDir)
                //currTime = 0;
            //}
        }
        //}

    }
}
