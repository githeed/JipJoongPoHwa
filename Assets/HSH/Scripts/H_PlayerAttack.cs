using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Transforms;
using UnityEngine;

public class H_PlayerAttack : MonoBehaviour
{
    //public List<GameObject> enemies = new List<GameObject>();

    public float attTime = 5;
    private float curAttTime = 0;

    public float scanRange = 10f;
    public LayerMask targetLayer;
    public Collider[] targets;
    public Transform nearestTarget;

    public float  attackDmg = 5f;

    public float maxHP = 1000;
    float curHP = 0;

    // Start is called before the first frame update
    void Start()
    {
        curHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        BasicAttack();
    }

    Transform GetNearest()
    {
        Transform result = null;
        float dist = 9999f;

        //Stopwatch stopwatch = new Stopwatch();
        //stopwatch.Start();


        // 구에 오버랩된 게임오브젝트 중에 가장 가까운 놈의 위치 찾기
        foreach(Collider target in targets)
        {
            float curDist = Vector3.Distance(transform.position, target.transform.position);
            if(curDist < dist)
            {
                dist = curDist;
                result = target.transform;
            }
        }
        //print(stopwatch.ElapsedMilliseconds);
        //print(stopwatch.Elapsed);
        //print(stopwatch.ElapsedTicks);
        //stopwatch.Stop();
        return result;
    }

    void BasicAttack()
    {
        // 5초마다
        curAttTime += Time.deltaTime;
        if(curAttTime > attTime)
        {
            // 오버랩 스피어
            targets = Physics.OverlapSphere(transform.position, scanRange, targetLayer);
            nearestTarget = GetNearest();

            if (nearestTarget == null) return;
            // 공격하기
            else
            //if (nearestTarget.gameObject != null)
            {
                nearestTarget.gameObject.GetComponent<EnemyMove>().UpdateHp(attackDmg);
                //print(nearestTarget.gameObject + ": " + attackDmg);
                //ObjectPoolManager.instance
            }
            curAttTime = 0;
        } 
    }

    public void UpdateHp(float dmg)
    {
        curHP -= dmg;
        print(curHP);
        if (curHP <= 0)
        {
            Destroy(gameObject);
        }
    }
}
