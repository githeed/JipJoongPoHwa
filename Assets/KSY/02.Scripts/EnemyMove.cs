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
    PlayerTest_K playerCs;
    public float attackPower;

    bool canAttack;
    NavMeshAgent agent;
    Coroutine attackCoroutine;

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
        playerCs = target.GetComponent<PlayerTest_K>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        
    }


    // Start is called before the first frame update
    void Start()
    {
        //OnNav();
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
        agent.destination = target.transform.position;
    }

    //private void OnEnable()
    //{
    //    rand = Random.Range(distanceMin, distanceMax);
    //    randDirX = Random.Range(-1f, 1f);
    //    randDirZ = Random.Range(-1f, 1f);
    //    while (randDirX == 0 && randDirZ == 0)
    //    {
    //        randDirX = Random.Range(-1f, 1f);
    //        randDirZ = Random.Range(-1f, 1f);
    //    }
    //    dir = new Vector3(randDirX, 0, randDirZ);
    //    transform.position = target.transform.position + (dir.normalized * rand);
    //    print("소환된 거리" + (transform.position - target.transform.position).magnitude);
    //}

    private void OnTriggerEnter(Collider other)
    {
        print("뭐든 만남.");
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            print("만남");
            canAttack = true;
            attackCoroutine = StartCoroutine(Attack()); // Coroutine형으로 받아서 참조하여 관리.
            agent.enabled = false;
            pool.Release(this.gameObject);
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

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {

            pool.Release(this.gameObject);
        }
    }*/

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
