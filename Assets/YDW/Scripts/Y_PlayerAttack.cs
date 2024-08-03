using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Entities.UniversalDelegates;
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
    public float PSkillDuration;
    public float curBAttTime = 0;
    public float curEAttTime = 0;
    public float curRAttTime = 0;
    public float curPAttTime = 0;
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
    public GameObject featherEFactory;
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

    public float featherSpeed = 10;

    Y_NavMesh allyNavMesh;

    void Start()
    {
        hp = GetComponent<Y_HPSystem>();

        featherTime = 10;
        basicAttTime = 2;
        ESkillTime = 9; 
        RSkillTime = 30; 
        PSkillDuration = 15;

        featherDist = 7;
        featherEftTime = 0.3f;
        attackDmg = 5f;
        eAttRate = 1.2f;
        enmStopTime = 1.5f;

        featherRNo = 24;

        AllyFSM = GetComponent<Y_AllyFSM>();
        allyNavMesh = GetComponent<Y_NavMesh>();

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

        ////////////////////////////
        if (!hp.isDead)
        {
            BasicAttack();
            ESkill();
            RSkill();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EvolvedWeapon();
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
                    hitinfo.transform.GetComponent<EnemyHp>().UpdateHp(attackDmg * eAttRate);
                    StartCoroutine(StopEnemy(hitinfo));
                }

                
                Destroy(feather.gameObject);

                curEAttTime = 0;

                

            }

            if (feathers.Length >= 30 && !pAttacking)
            {
                StartCoroutine(PassiveAttack());
                StartCoroutine(SkillTimeFast());

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
                    hitinfo.transform.GetComponent<EnemyHp>().UpdateHp(attackDmg);
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

    void EvolvedWeapon()
    {
        StartCoroutine(EvolveCrt());
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
                
                hitinfo.transform.GetComponent<EnemyHp>().UpdateHp(attackDmg);

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
        allyNavMesh.moveSpeed *= 2;
        yield return new WaitForSecondsRealtime(2f);
        unbeatable = false;
        allyNavMesh.moveSpeed /= 2;
    }

    Vector3 p1;
    Vector3 p2;
    Vector3 p3;
    Vector3 p4;
    Vector3 p5;
    Vector3 p6;
    Vector3 p7;
    Vector3 p8;
    Vector3 dir;

    private IEnumerator PassiveAttack()
    {
        curPAttTime = 0;
        pAttacking = true;
        

        while (curPAttTime < PSkillDuration)
        {

           // curPAttTime += Time.deltaTime;

            p1 = transform.position;
            p2 = transform.position + 3f * transform.right;// + transform.forward * -10f;
            p3 = transform.position - 3f * transform.right;

            Transform targetP = GetNearest();
            p4 = targetP.position;

            if (targetP == null)
            {
                UnityEngine.Debug.LogError("targetP is null");
                continue;
            }
            

            for (int i = 0; i < basicAttackNo; i++)
            {
                float time = 0f;

                GameObject feather = Instantiate(featherFactory);
                feather.transform.position = transform.position;
                feather.layer = LayerMask.NameToLayer("PassiveFeather");
                GameObject feather2 = Instantiate(featherFactory);
                feather2.transform.position = transform.position;
                feather2.layer = LayerMask.NameToLayer("PassiveFeather");

                dir = p4 - transform.position;

                if(i == 0)
                {
                    // 몸통 회전시키기
                    Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
                    transform.rotation = rotation;
                }

                while (true)
                {

                    dir = p4 - transform.position;

                    time += Time.deltaTime * 7;

                    time = Mathf.Clamp(time, 0, 1);

                    p5 = Vector3.Lerp(p1, p2, time);
                    p6 = Vector3.Lerp(p2, p4, time);

                    p7 = Vector3.Lerp(p1, p3, time);
                    p8 = Vector3.Lerp(p3, p4, time);

                    if(feather== null)
                    {
                        print("111");
                    }
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

                yield return null;

            }

            if(targetP != null && targetP.GetComponent<EnemyMove>() != null)
            {
                targetP.GetComponent<EnemyHp>().UpdateHp(attackDmg * batRate * basicAttackNo);
            }
            yield return new WaitForSecondsRealtime(1f);
            curPAttTime += 1;
        }

        pAttacking = false;
    }

    private IEnumerator SkillTimeFast()
    {
        skillTimeRate = 1.5f;
        yield return new WaitForSecondsRealtime(15f);
        skillTimeRate = 1f;
    }

    private IEnumerator EvolveCrt()
    {
        GameObject feather = Instantiate(featherEFactory);
        feather.transform.position = transform.position;

        List<(Vector3 dir, Collider collider)> targets = new List<(Vector3 dir, Collider collider)>();

        // OverlapSphere 로 근처에 있는 에너미들 정보 가져오고
        Collider[] hitInfos = Physics.OverlapSphere(transform.position, scanRange, targetLayer);
        // 리스트에 추가해줌
        foreach (Collider hitInfo in hitInfos)
        {
            Vector3 dirToTarget = hitInfo.gameObject.transform.position - transform.position;
            targets.Add((dirToTarget, hitInfo));
        }
        // 리스트 거리 순으로 소팅
        var enemies = targets.OrderBy(target => target.dir.magnitude).ToList();

        for (int i = 0; i < 4; i++)
        {
            if (i >= enemies.Count || enemies[i].collider == null)
            {
                print("yield break???????");
                Destroy(feather);
                yield break;
            }
            else
            {
                Vector3 dirEW;
                Vector3 destination;


                float dist = 0;
                float nextDist = dist - 1;
                while (dist > nextDist)
                {
                    if (i < 3)
                    {
                        destination = enemies[i].collider.transform.position;
                    }
                    else
                    {
                        destination = transform.position;
                        
                    }

                    dirEW = destination - feather.transform.position;
                    dist = Vector3.Distance(feather.transform.position, destination);
                    feather.transform.position += dirEW.normalized * featherSpeed * Time.deltaTime;
                    nextDist = Vector3.Distance(feather.transform.position, destination);


                    ////// 나중에 break
                    if (enemies[i].collider == null)
                    {
                        print("yield break??????? 2222222222222222");
                        Destroy(feather);
                        yield break;
                    }


                    yield return null;


                }
     
                enemies[i].collider.gameObject.GetComponent<EnemyHp>().UpdateHp(attackDmg);

            }

            if(i == 3)
            {
                Destroy(feather);
                hp.Heal(hp.maxHealth * 0.1f * i);
            }
        }
        //yield return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p4);
        
    }



}
