using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class EnemyMove : MonoBehaviour
{
    public IObjectPool<GameObject> pool { get; set; }
    GameObject target;
    H_PlayerAttack playerCs;
    public float attackPower;

    bool canAttack;
    NavMeshAgent agent;
    Coroutine attackCoroutine;

    public float maxHp;
    float curHp;


    public float distanceMin;
    public float distanceMax;
    Vector2 distance;
    float rand;
    float randDirX;
    float randDirZ;
    Vector3 dir;

    private void Awake()
    {
        target = GameObject.FindWithTag("Player");
        playerCs = target.GetComponent<H_PlayerAttack>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        curHp = maxHp;
    }




    public void OnNav()
    {
        StartCoroutine(Co_OnNav());
    }

    IEnumerator Co_OnNav()
    {
        yield return null;

        agent.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(agent.enabled)
        agent.destination = target.transform.position;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            canAttack = true;
            attackCoroutine = StartCoroutine(Attack()); // Coroutine형으로 받아서 참조하여 관리.
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            StopCoroutine(attackCoroutine); // 코루틴을 하나만 할 수 있게.
            canAttack = false;
            
        }
    }

    public void UpdateHp(float dmg)
    {
        curHp -= dmg;
        if(curHp <= 0)
        {
            print(curHp);
            agent.enabled = false;
            pool.Release(this.gameObject);
        }
    }

    IEnumerator Attack()
    {
        while (canAttack)
        {
            playerCs.UpdateHp(attackPower);
            yield return new WaitForSeconds(1.0f);
        }
    }

}
