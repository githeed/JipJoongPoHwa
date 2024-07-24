using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    GameObject target;
    PlayerTest_K playerCs;
    public float attackPower;

    bool canAttack;
    NavMeshAgent agent;


    Coroutine attackCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindWithTag("Player");
        playerCs = target.GetComponent<PlayerTest_K>();
        agent = GetComponent<NavMeshAgent>();
        
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = target.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        print("뭐든 만남.");
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            print("만남");
            canAttack = true;
            attackCoroutine = StartCoroutine(Attack()); // Coroutine형으로 받아서 참조하여 관리.
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            print("그만 때림");
            StopCoroutine(attackCoroutine); // 코루틴을 하나만 할 수 있게.
            canAttack = false;
        }
    }


    IEnumerator Attack()
    {
        while (canAttack)
        {
            print("때림");
            playerCs.UpdateHP(attackPower);
            yield return new WaitForSeconds(1.0f);
        }
    }

}
