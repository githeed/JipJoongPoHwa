using NUnit.Framework;
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
    GameObject player0;
    GameObject player1;
    GameObject target;
    float dist0;
    float dist1;

    H_PlayerAttack playerCsH;
    Y_PlayerAttack playerCsY;
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

    public string[] debug = new string[10];


    private void Awake()
    {
        player0 = GameObject.FindWithTag("Player");
        player1 = GameObject.FindWithTag("Player1");
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
    }

    private void OnEnable()
    {
        curHp = maxHp;
    }

    void Update()
    {
        dist0 = (player0.transform.position - transform.position).magnitude;
        if(player1 != null && player1.activeSelf)
        {
            dist1 = (player1.transform.position - transform.position).magnitude;
            if (dist0 < dist1)
            {
                target = player0;
            }
            else
            {
                target = player1;
            }
        }
        if (player1 == null) target = player0;
        


        Vector2 forward = new Vector2(transform.position.z, transform.position.x);
        Vector2 steeringTarget = new Vector2(agent.steeringTarget.z, agent.steeringTarget.x);

        // 방향을 구한 뒤, 역함수로 각을 구함.
        Vector2 dir = steeringTarget - forward;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // 방향적용
        transform.eulerAngles = Vector3.up * angle;

        if (player1 != null && !player1.activeSelf && player0 != null) target = player0;
        if(agent.enabled && target != null)
        agent.destination = target.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            canAttack = true;
            attackCoroutine = StartCoroutine(Attack()); // Coroutine형으로 받아서 참조하여 관리.
            if (target != null)
            {
                playerCsH = other.GetComponent<H_PlayerAttack>();
                playerCsY = other.GetComponent<Y_PlayerAttack>();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            StopCoroutine(attackCoroutine); // 코루틴을 하나만 할 수 있게.
            canAttack = false;
            playerCsY = null;
            playerCsH = null;
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
            if(playerCsH != null)
            {
                playerCsH.UpdateHp(attackPower);
            }
            if(playerCsY != null)
            {
                playerCsY.UpdateHp(attackPower);
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

}
