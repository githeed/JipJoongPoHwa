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

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
        ally = GameObject.Find("Ally");
        allyFSM = ally.GetComponent<Y_AllyFSM>();
        yp = ally.GetComponent<Y_PlayerAttack>();
    }
    void Start()
    {
        agent.speed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        float distToTarget = Vector3.Distance(player.transform.position, transform.position);

        if(distToTarget > defaultDist)
        {
            agent.destination = (player.transform.position);
        }
        else
        {
            agent.destination = (transform.position + allyFSM.moveDir);
        }

    }
}
