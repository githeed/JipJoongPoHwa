using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class EnemyMove : MonoBehaviour
{
    public IObjectPool<GameObject> pool { get; set; }
    GameObject[] targets;
    GameObject target;
    List<float> distanceToTargets = new List<float>();
    H_PlayerAttack playerCs;
    public float attackPower;

    bool canAttack;
    Coroutine attackCoroutine;
    NavMeshAgent agent;

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
        targets = GameObject.FindGameObjectsWithTag("Player");
        for(int i = 0; i < targets.Length; i++)
        {
            distanceToTargets.Add((targets[i].transform.position - transform.position).magnitude);
        }
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        curHp = maxHp;
    }

    void Update()
    {
        print("Found " + targets.Length + " players.");
        if(targets.Length == 1) // target이 한명이면
        {
            target = targets[0];
        }
        if(targets.Length >= 2) // target이 둘 이상이면
        {
            for(int i = 0; i < targets.Length; i++)
            {
                distanceToTargets[i] = (targets[i].transform.position - transform.position).magnitude;
            }
            for(int i = 0; i < targets.Length; i++)
            {
                if (distanceToTargets[i] == distanceToTargets.Min())
                {
                    target = targets[i];
                }
            }
        }
        if(target != null)
        {
            playerCs = target.GetComponent<H_PlayerAttack>();
        }
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

    /// <summary>
    /// 매개변수에 float형으로 공격력(attackPower)넣어주면 됨.
    /// </summary>
    /// <param name="dmg"></param>
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
    public void OnNav()
    {
        StartCoroutine(Co_OnNav());
    }

    IEnumerator Co_OnNav()
    {
        yield return null;
        agent.enabled = true;
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
