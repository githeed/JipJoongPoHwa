using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Transforms;
using UnityEngine;

public class Y_PlayerAttack : MonoBehaviour
{

    public float attTime = 5;
    private float curAttTime = 0;

    public float scanRange = 10f;
    public LayerMask targetLayer;
    public Collider[] targets;
    public Transform nearestTarget;

    public float attackDmg = 5f;

    Y_HPSystem hp;

    public GameObject featherFactory;
    public GameObject basicAttEffFactory;
    public float basicAttackNo = 3;
    public float featherDist;
    public float featherEftTime;
    public float featherTime;


 
    void Start()
    {
        hp = GetComponent<Y_HPSystem>();
        featherDist = 7;
        featherEftTime = 0.3f;
        featherTime = 10;
    }


    void Update()
    {
        if(!hp.isDead)
        {
            BasicAttack();

        }
    }

    Transform GetNearest()
    {
        Transform result = null;
        float dist = 9999f;

        // 구에 오버랩된 게임오브젝트 중에 가장 가까운 놈의 위치 찾기
        foreach (Collider target in targets)
        {
            float curDist = Vector3.Distance(transform.position, target.transform.position);
            if (curDist < dist)
            {
                dist = curDist;
                result = target.transform;
            }
        }

        return result;
    }

    void BasicAttack()
    {
        // 5초마다
        curAttTime += Time.deltaTime;
        if (curAttTime > attTime)
        {
            // 오버랩 스피어
            targets = Physics.OverlapSphere(transform.position, scanRange, targetLayer);
            nearestTarget = GetNearest();

            if (nearestTarget == null) return;
            // 공격하기
            else
            {

                StartCoroutine(FeatherAttack());

                nearestTarget.gameObject.GetComponent<EnemyMove>().UpdateHp(attackDmg);
            }
            curAttTime = 0;
        }
    }

    public void UpdateHp(float dmg)
    {
        hp.Damaged(dmg);
    }

    private IEnumerator FeatherAttack()
    {
        int i = 0;

        Vector3 dirFrAllyToEnm = nearestTarget.transform.position - transform.position; // 처음 깃털 한 번만 해야 한다
        dirFrAllyToEnm.y = 0;
        Vector3 dirFrAllyToEnmNor = dirFrAllyToEnm.normalized;

        while (i < basicAttackNo)
        {
            
            GameObject basicAttEff = Instantiate(basicAttEffFactory);
            basicAttEff.transform.forward = dirFrAllyToEnm;
            basicAttEff.transform.position = transform.position;

            Destroy(basicAttEff, featherEftTime);

            GameObject feather = Instantiate(featherFactory);
            feather.transform.position = transform.position + featherDist * dirFrAllyToEnmNor; 

            Destroy(feather, featherTime);

            i++;
            yield return new WaitForSecondsRealtime(featherEftTime);
        }

    }
}
