using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using static UnityEngine.GraphicsBuffer;

public class Y_PlayerAttack : MonoBehaviour
{
    // Time
    public float basicAttTime;
    public float ESkillTime;
    public float RSkillTime;
    public float curBAttTime = 0;
    public float curEAttTime = 0;
    public float curRAttTime = 0;
    public float skillTimeRate = 1;

    // Scan and Target
    public float scanRange = 10f;
    public LayerMask targetLayer;
    public LayerMask featherLayer;
    public Collider[] targets;
    public Collider[] feathers;
    public GameObject[] feathersE;
    public GameObject[] feathersR;
    public Transform nearestTarget;

    // AttackDmg and HP
    public float attackDmg;
    Y_HPSystem hp;

    // Feather Attack
    public GameObject featherFactory;
    public GameObject basicAttEffFactory;
    public float basicAttackNo = 3;
    public float featherDist;
    public float featherTime;

    public float featherEftTime;
    public float eAttRate;
    public float enmStopTime;

    public float featherRNo;

    public bool unbeatable = false;
    Y_AllyFSM AllyFSM;

    public bool pAttacking = false;
    public float batRate = 1.05f;

    void Start()
    {
        hp = GetComponent<Y_HPSystem>();

        featherTime = 10;
        basicAttTime = 2;
        ESkillTime = 9;
        RSkillTime = 30;

        featherDist = 7;
        featherEftTime = 0.3f;
        attackDmg = 5f;
        eAttRate = 1.2f;
        enmStopTime = 1.5f;

        featherRNo = 24;

        AllyFSM = GetComponent<Y_AllyFSM>();


    }


    public RFX4_EffectSettings[] allEffects; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            for(int i = 0; i < allEffects.Length; i++)
            {
                allEffects[i].gameObject.SetActive(true);
                allEffects[i].Fire();
            }
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            for (int i = 0; i < allEffects.Length; i++)
            {
                allEffects[i].gameObject.SetActive(false);
            }
        }

        ////////////////////////////////
        //if (!hp.isDead)
        //{
        //    BasicAttack();
        //    ESkill();
        //    RSkill();
        //}

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(PassiveAttack());
            StartCoroutine(SkillTimeFast());
        }


        
    }

    Transform GetNearest()
    {
        Transform result = null;
        float dist = 9999f;

        targets = Physics.OverlapSphere(transform.position, scanRange, targetLayer);

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
        curBAttTime += Time.deltaTime * skillTimeRate;
        if (curBAttTime > basicAttTime)
        {
            // 오버랩 스피어
           // targets = Physics.OverlapSphere(transform.position, scanRange, targetLayer);
            nearestTarget = GetNearest();

            if (nearestTarget == null) return;
            // 공격하기
            else
            {
                StartCoroutine(FeatherAttack());
            }
            curBAttTime = 0;
        }
    }

    void ESkill()
    {
        curEAttTime += Time.deltaTime * skillTimeRate;
        if(curEAttTime > ESkillTime)
        {
            // 범위 안의 깃털들 정보 가져와서
            feathers = Physics.OverlapSphere(transform.position, 99999f, featherLayer);

            //int i = 0;
            foreach (Collider feather in feathers)
            {
                // Ally 에게 깃털 모아지는 파티클
                Vector3 dirFrFthToAlly = transform.position - feather.gameObject.transform.position;
                dirFrFthToAlly.y = 0;
                Vector3 dirFrFthToAllyNor = dirFrFthToAlly.normalized;
                FeatherParticle(feather.gameObject, dirFrFthToAlly);

                RaycastHit[] hitInfos = Physics.RaycastAll(feather.transform.position + Vector3.up * 0.5f, dirFrFthToAlly, dirFrFthToAlly.magnitude, targetLayer);

                foreach (RaycastHit hitinfo in hitInfos)
                {
                    //i++;
                    //print(feathers.Length + ", " + i);
                    hitinfo.transform.GetComponent<EnemyMove>().UpdateHp(attackDmg * eAttRate);
                    StartCoroutine(StopEnemy(hitinfo));
                }


                Destroy(feather.gameObject);

                curEAttTime = 0;

            }
        }
        
    }

    void RSkill()
    {
        curRAttTime += Time.deltaTime * skillTimeRate;
        if (curRAttTime > RSkillTime)
        {
            for (int i = 1; i <= featherRNo; i++)
            {
                // 깃털 360도로 퍼지게
                GameObject feather = Instantiate(featherFactory);
                feather.transform.Rotate(0, (360 / featherRNo) * i, 0);
                feather.transform.position = transform.position + featherDist * feather.transform.forward;

                // Enemy 에게 데미지 주기
                RaycastHit[] hitInfos = Physics.RaycastAll(transform.position + Vector3.up * 0.5f, feather.transform.forward, featherDist, targetLayer);
                foreach (RaycastHit hitinfo in hitInfos)
                {
                    hitinfo.transform.GetComponent<EnemyMove>().UpdateHp(attackDmg);
                }

                // 파티클 생성
                FeatherParticle(gameObject, feather.transform.forward);

                // 무적 상태 만들기
                StartCoroutine(Unbeatable());

                // 시간 지나면 깃털 파괴
                Destroy(feather, featherTime);

                
            }
        
            curRAttTime = 0;
        }
    }

    public void UpdateHp(float dmg)
    {
        if (unbeatable) return;
        hp.Damaged(dmg);
    }

    void FeatherParticle(GameObject obj, Vector3 dir)
    {
        GameObject basicAttEff = Instantiate(basicAttEffFactory);
        basicAttEff.transform.forward = dir;
        basicAttEff.transform.position = obj.transform.position;
        Destroy(basicAttEff, featherEftTime);
    }

    public void RemoveFeather()
    {
        feathers = Physics.OverlapSphere(transform.position, 99999f, featherLayer);

        foreach (Collider feather in feathers)
        {
            Destroy(feather.gameObject);
        }
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
            FeatherParticle(gameObject, dirFrAllyToEnm);

            RaycastHit[] hitInfos = Physics.RaycastAll(transform.position + Vector3.up * 0.5f, dirFrAllyToEnm, featherDist, targetLayer);
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

    private IEnumerator StopEnemy(RaycastHit hitinfo)
    {
        hitinfo.transform.GetComponent<NavMeshAgent>().enabled = false;
        yield return new WaitForSecondsRealtime(enmStopTime);
        hitinfo.transform.GetComponent<NavMeshAgent>().enabled = true;
    }

    private IEnumerator Unbeatable()
    {
        unbeatable = true;
        AllyFSM.moveSpeed *= 2;
        yield return new WaitForSecondsRealtime(2f);
        unbeatable = false;
        AllyFSM.moveSpeed /= 2;
    }

    Vector3 p1;
    Vector3 p2;
    Vector3 p3;
    Vector3 p4;
    Vector3 p5;
    Vector3 p6;
    Vector3 p7;
    Vector3 p8;

    private IEnumerator PassiveAttack()
    {
        

        p1 = transform.position;
        p2 = transform.position + 3f * transform.right;// + transform.forward * -10f;
        p3 = transform.position - 3f * transform.right;

        Transform targetP = GetNearest();

        if (targetP == null)
        {
            UnityEngine.Debug.LogError("targetP is null");
            //break;
        }

        p4 = targetP.transform.position;

        Vector3 dir = p4 - transform.position;

        int i = 0;
        while (i < basicAttackNo)
        {
            float time = 0f;

            GameObject feather = Instantiate(featherFactory);
            feather.transform.position = transform.position;
            GameObject feather2 = Instantiate(featherFactory);
            feather2.transform.position = transform.position;

            

            while (true)
            {
                

                time += Time.deltaTime * 7;

                time = Mathf.Clamp(time, 0, 1);

                p5 = Vector3.Lerp(p1, p2, time);
                p6 = Vector3.Lerp(p2, p4, time);

                p7 = Vector3.Lerp(p1, p3, time);
                p8 = Vector3.Lerp(p3, p4, time);

                feather.transform.position = Vector3.Lerp(p5, p6, time);
                feather2.transform.position = Vector3.Lerp(p7, p8, time);

                if (time >= 1)
                {
                    Destroy(feather);
                    Destroy(feather2);
                    break;
                }
                yield return null;


            }

            // 몸통 회전시키기

            Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = rotation;
            yield return null;
            i++;


        }

        
    }

    private IEnumerator SkillTimeFast()
    {
        skillTimeRate = 1.5f;
        yield return new WaitForSecondsRealtime(15f);
        skillTimeRate = 1f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p4);
        
    }



}
