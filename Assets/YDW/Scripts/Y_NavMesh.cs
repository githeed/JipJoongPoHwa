using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Y_NavMesh : MonoBehaviour
{
    NavMeshAgent agent;
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
        //float distToTarget = Vector3.Distance(player.transform.position, transform.position);

        //if(distToTarget > defaultDist)
        //{
        //    agent.destination = (player.transform.position);
        //}
        //else
        //{
        if (GameManager.instance.canPick) bossExist = true;
        if (bossExist) bossCs = GameManager.instance.bossCs;
        if (bossCs != null && bossCs.stone.activeSelf)
        {
            //if (GameManager.instance.bossSpawn && bossCs2 == null)
            //{
            //    bossCs2 = GameObject.FindWithTag("Boss").GetComponent<Boss>();
            //}
            //bossCs2.stone
            //GameObject go = Instantiate("")
            //agent.destination = bossStone.transform.position - (playerAttack.featherDist - 5) * bossStone.transform.forward;
            agent.destination = bossStone.transform.position + Vector3.right;
        }
        else if (boss.activeSelf)
        {
            agent.destination = boss.transform.position - (playerAttack.featherDist - 5) * Vector3.forward;
        }
        else if(!playerAttack.isESkill && !playerAttack.isBAttack && !playerAttack.isRSkill)
        {
            agent.destination = (transform.position + allyFSM.moveDir);
        }
        //}

    }
}
