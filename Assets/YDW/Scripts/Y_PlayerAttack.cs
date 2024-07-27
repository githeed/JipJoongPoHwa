using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Transforms;
using UnityEditor.PackageManager;
using UnityEngine;

public class Y_PlayerAttack : MonoBehaviour
{

    public float attTime;
    private float curAttTime = 0;

    public float scanRange = 10f;
    public LayerMask targetLayer;
    public LayerMask featherLayer;
    public Collider[] targets;
    public Collider[] feathers;
    public Transform nearestTarget;

    public float attackDmg;

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
        attackDmg = 10f;
        attTime = 2;

    }


    void Update()
    {
        if(!hp.isDead)
        {
            BasicAttack();
            if(Input.GetKeyDown(KeyCode.E))
            {
                ESkill();
            }

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
            }
            curAttTime = 0;
        }
    }

    void ESkill()
    {
        // 범위 안의 깃털들 정보 가져와서
        feathers = Physics.OverlapSphere(transform.position, 100000f, featherLayer);

        foreach (Collider feather in feathers)
        {
            // Ally 에게 깃털 모아지는 파티클
            Vector3 dirFrFthToAlly = transform.position - feather.gameObject.transform.position;
            dirFrFthToAlly.y = 0;
            Vector3 dirFrFthToAllyNor = dirFrFthToAlly.normalized;

            GameObject basicAttEff = Instantiate(basicAttEffFactory);
            basicAttEff.transform.forward = dirFrFthToAlly;
            basicAttEff.transform.position = feather.gameObject.transform.position;
            Destroy(basicAttEff, featherEftTime);

            RaycastHit[] hitInfos = Physics.RaycastAll(feather.transform.position, dirFrFthToAlly, dirFrFthToAlly.magnitude, targetLayer);
            

            foreach (RaycastHit hitinfo in hitInfos)
            {
                hitinfo.transform.GetComponent<EnemyMove>().UpdateHp(attackDmg);
            }


            Destroy(feather.gameObject);

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
            // 쏘아지는 이펙트 만들고 파괴
            GameObject basicAttEff = Instantiate(basicAttEffFactory);
            basicAttEff.transform.forward = dirFrAllyToEnm;
            basicAttEff.transform.position = transform.position;
            Destroy(basicAttEff, featherEftTime);


            RaycastHit[] hitInfos = Physics.RaycastAll(transform.position, dirFrAllyToEnm, featherDist, targetLayer);
            int nth = 0;
            foreach (RaycastHit hitinfo in hitInfos)
            {
                // 1: 0%, 2: 15%, 3:30%, 4: 45%, 5:60%
                if (nth < 5)
                {
                    attackDmg = attackDmg * (1 - ((i * 15)/ 100));
                }
                else
                {
                    attackDmg *= 0.4f;
                }
                nth++;

                hitinfo.transform.GetComponent<EnemyMove>().UpdateHp(attackDmg);

                
                
            }

            // 깃털 만들고 파괴
            GameObject feather = Instantiate(featherFactory);
            feather.transform.position = transform.position + featherDist * dirFrAllyToEnmNor; 
            Destroy(feather, featherTime);

            i++;
            yield return new WaitForSecondsRealtime(featherEftTime);
        }

    }

    //private IEnumerator GatherFeather()
    //{

    //    // 범위 안의 깃털들 정보 가져와서
    //    feathers = Physics.OverlapSphere(transform.position, 100f, featherLayer);
    //    List<GameObject> feathersToDestroy = new List<GameObject>();
        
    //    foreach(Collider feather in feathers)
    //    {
    //        // Ally 에게 깃털 모아지는 파티클
    //        Vector3 dirFrFthToAlly = transform.position - feather.gameObject.transform.position;
    //        dirFrFthToAlly.y = 0;
    //        Vector3 dirFrFthToAllyNor = dirFrFthToAlly.normalized;

    //        GameObject basicAttEff = Instantiate(basicAttEffFactory);
    //        basicAttEff.transform.forward = dirFrFthToAlly;
    //        basicAttEff.transform.position = feather.gameObject.transform.position;
    //        Destroy(basicAttEff, featherEftTime);

    //        RaycastHit[] hitInfos = Physics.RaycastAll(feather.transform.position, dirFrFthToAlly, dirFrFthToAlly.magnitude, featherLayer);

    //        foreach(RaycastHit hitinfo in hitInfos)
    //        {
    //            hitinfo.transform.GetComponent<EnemyMove>().UpdateHp(attackDmg);
    //        }

    //        // 나중에 파괴하기 위해 리스트에 깃털 추가
    //        feathersToDestroy.Add(feather.gameObject);
    //    }

    //    foreach (GameObject feather in feathersToDestroy)
    //    {
    //        Destroy(feather);
    //    }

    //    yield return null;
    //}


}
