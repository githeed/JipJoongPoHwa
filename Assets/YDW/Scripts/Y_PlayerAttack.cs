using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Entities.UniversalDelegates;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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
    public float scanRange = 50f;
    public LayerMask targetLayer;
    public LayerMask featherLayer;
    public Collider[] targets;
    //public Collider[] feathers;
    public GameObject[] feathersE;
    public GameObject[] feathersR;
    //public Transform nearestTarget;

    // AttackDmg and HP
    public float attackDmg;
    Y_HPSystem hp;

    // Feather Attack
    public GameObject featherFactory;
    public GameObject featherEFactory;
    public GameObject featherAFactory;
    public GameObject basicAttEffFactory;
    public float basicAttackNo = 3;
    public float featherDist;
    public float featherTime;


    public float eAttRate;
    public float enmStopTime;

    public float featherRNo;

    public bool unbeatable = false;
    Y_AllyFSM AllyFSM;

    public bool pAttacking = false;
    public float batRate = 1.05f;

    public float featherSpeedB;
    public float featherSpeedE;
    public float featherSpeed = 10;

    Y_NavMesh allyNavMesh;

    Animator anim;

    public bool isBAttack = false;
    public bool isESkill = false;
    public bool isRSkill = false;

    GameObject manager;
    H_PlayerManager pm;

    public GameObject damageParticle;

    public GameObject allyBody;

    public GameObject nearestTargetB;

    public IObjectPool<GameObject> pool { get; set; }

    private bool isFeatherReleased = false;

    void Start()
    {
        hp = GetComponent<Y_HPSystem>();

        featherTime = 10;
        basicAttTime = 2;
        ESkillTime = 9; 
        RSkillTime = 9999; 
        PSkillDuration = 15;

        featherDist = 7;
        attackDmg = 5f;
        eAttRate = 1.2f;
        enmStopTime = 1.5f;

        featherRNo = 24;

        featherSpeedB = 50;
        featherSpeedE = 100;

        AllyFSM = GetComponent<Y_AllyFSM>();
        allyNavMesh = GetComponent<Y_NavMesh>();
        anim = GetComponentInChildren<Animator>();

        manager = GameObject.Find("H_PlayerManager");
        pm = manager.GetComponent<H_PlayerManager>();
        allyBody = GameObject.Find("AllyBody");

        nearestTargetB = GameObject.Find("Enemy");

    }


    public RFX4_EffectSettings[] allEffects; 

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    for(int i = 0; i < allEffects.Length; i++)
        //    {
        //        allEffects[i].gameObject.SetActive(true);
        //        allEffects[i].Fire();
        //    }
        //}

        //if (Input.GetKeyDown(KeyCode.Y))
        //{
        //    for (int i = 0; i < allEffects.Length; i++)
        //    {
        //        allEffects[i].gameObject.SetActive(false);
        //    }
        //}

        ////////////////////////////
        if (!hp.isDead)
        {

            curBAttTime += Time.deltaTime * skillTimeRate;
            if (curBAttTime > basicAttTime && !isESkill && !isRSkill)
            {
                BasicAttack();
                curBAttTime = 0;
            }

            curEAttTime += Time.deltaTime * skillTimeRate;
            if (curEAttTime > ESkillTime && !isBAttack && !isRSkill) 
            {
                StartCoroutine(ESkill());
                curEAttTime = 0;
            }

            curRAttTime += Time.deltaTime * skillTimeRate;
            if (curRAttTime > RSkillTime && !isBAttack && !isESkill)
            {
                StartCoroutine(RSkill());
                curRAttTime = 0;
            }
        }

        

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            StartCoroutine(EvolveCrt());
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            StartCoroutine(RSkill());
        }






        // 레벨별로 속도나 개수 변경하기
        if (pm.indexLev == 1)
        {
            basicAttTime = 2;
            ESkillTime = 9;
        }
        else if(pm.indexLev == 2)
        {
            basicAttTime = 1.7f;
            ESkillTime = 8;
        }
        else if (pm.indexLev == 3)
        {
            basicAttTime = 1.5f;
            ESkillTime = 7;
        }
        else if (pm.indexLev == 4)
        {
            basicAttTime = 1.3f;
            ESkillTime = 6;
        }
        else
        {
            basicAttTime = 1f;
            ESkillTime = 5;
        }

        if (pm.indexLev > 5)
        {
            RSkillTime = 30;
        }

        batRate = 1.05f + 0.01f * pm.indexLev;


    }

    public Transform GetNearest()
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

    public Vector3 dirB;
    void BasicAttack()
    {
        isBAttack = true;
        // 오버랩 스피어
        // targets = Physics.OverlapSphere(transform.position, scanRange, targetLayer);
        nearestTargetB = GetNearest().gameObject;

        if (nearestTargetB == null) return;
        // 공격하기
        else
        {
            StartCoroutine(FeatherAttack());
        }
        dirB = nearestTargetB.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(dirB, Vector3.up);
        allyBody.transform.rotation = rotation;

        curBAttTime = 0;

    }


    private IEnumerator ESkill()
    {
        isESkill = true;
        anim.SetTrigger("ESKILL");
        yield return new WaitForSecondsRealtime(0.6f);

        Collider[] feathers = Physics.OverlapSphere(transform.position, 99999f, featherLayer);
        
        foreach (Collider feather in feathers)
        {
            Vector3 dirFrFthToAlly = transform.position - feather.gameObject.transform.position;
            dirFrFthToAlly.y = 0;
            Vector3 dirFrFthToAllyNor = dirFrFthToAlly.normalized;

            Vector3 destinationE = transform.position;
            StartCoroutine(MoveFeather(feather, dirFrFthToAllyNor, destinationE, 0.1f));

            

            // 레이캐스트로 데미지
            RaycastHit[] hitInfos = Physics.RaycastAll(feather.transform.position + Vector3.up * 0.5f, dirFrFthToAlly, dirFrFthToAlly.magnitude, targetLayer);

            foreach (RaycastHit hitinfo in hitInfos)
            {
                //i++;
                //print(feathers.Length + ", " + i);

                hitinfo.transform.GetComponent<EnemyHp>().UpdateHp(attackDmg * eAttRate);
                DamageParticle(hitinfo.transform.position + Vector3.up);
                StartCoroutine(StopEnemy(hitinfo));
            }

                
                

        }

        if (feathers.Length >= 30 && !pAttacking)
        {
            StartCoroutine(PassiveAttack());
            StartCoroutine(SkillTimeFast());

        }

        curEAttTime = 0;
        isESkill = false;
        anim.SetTrigger("BLEND_TREE");
    }

    private IEnumerator RSkill()
    {
        isRSkill = true;
        anim.SetTrigger("RSKILL");

        // 무적 상태 만들기
        StartCoroutine(Unbeatable());

        yield return new WaitForSecondsRealtime(0.8f);

        for (int i = 1; i <= featherRNo; i++)
        {
            // 깃털 360도로 퍼지게
            isFeatherReleased = false;
            GameObject feather = ObjectPoolManager.instance.featherPool.Get();
            feather.transform.position = transform.position;
            feather.transform.Rotate(0, (360 / featherRNo) * i, 0);

            // Enemy 에게 데미지 주기
            RaycastHit[] hitInfos = Physics.RaycastAll(transform.position + Vector3.up * 0.5f, feather.transform.forward, featherDist, targetLayer);
            foreach (RaycastHit hitinfo in hitInfos)
            {
                hitinfo.transform.GetComponent<EnemyHp>().UpdateHp(attackDmg);
                DamageParticle(hitinfo.transform.position + Vector3.up);
            }

            // feather 움직이게
            Vector3 destinationR = transform.position + featherDist * feather.transform.forward;
            StartCoroutine(MoveFeather(feather.GetComponent<CapsuleCollider>(), feather.transform.forward, destinationR, featherTime));
            //FeatherParticle(gameObject, feather.transform.forward);

            // 시간 지나면 깃털 파괴
            if (feather != null) StartCoroutine(ReleaseFeather(feather, featherTime));
            //Destroy(feather, featherTime);

                
        }
        
        curRAttTime = 0;
        isRSkill = false;
        anim.SetTrigger("BLEND_TREE");

    }


    public void UpdateHp(float dmg)
    {
        if (unbeatable) return;
        hp.Damaged(dmg);
    }

    //void FeatherParticle(GameObject obj, Vector3 dir)
    //{
    //    GameObject basicAttEff = Instantiate(basicAttEffFactory);
    //    basicAttEff.transform.forward = dir;
    //    basicAttEff.transform.position = obj.transform.position;
    //    Destroy(basicAttEff, featherEftTime);
    //}

    public void RemoveFeather()
    {
        Collider[] feathers = Physics.OverlapSphere(transform.position, 99999f, featherLayer);

        foreach (Collider feather in feathers)
        {
            if (feather != null) StartCoroutine(ReleaseFeather(feather.gameObject, featherTime));
        }
    }

    void DamageParticle(Vector3 dir)
    {
        GameObject dp = Instantiate(damageParticle);
        dp.transform.position = dir;
    }


    // 기본 공격 (날개 달린 단검)
    private IEnumerator FeatherAttack()
    {


        //// dir 으로 바꾸기??????
        //Vector3 dirFrAllyToEnm = nearestTargetB.transform.position - transform.position; // 처음 깃털 한 번만 해야 한다
        //dirFrAllyToEnm.y = 0;
        //Vector3 dirFrAllyToEnmNor = dirFrAllyToEnm.normalized;

        int i = 0;
        anim.SetTrigger("BASIC_ATTACK");
        yield return new WaitForSecondsRealtime(0.3f);

        //Quaternion rotation = Quaternion.LookRotation(dirFrAllyToEnm, Vector3.up);
        //transform.rotation = rotation;
        //allyBody.transform.forward = dirFrAllyToEnm;
        allyBody.transform.forward = dirB;

        yield return new WaitForSecondsRealtime(0.4f);

        while (i < basicAttackNo)
        {
            // 깃털 생성 후 Ally 위치에 놓음
            //GameObject feather = Instantiate(featherFactory);
            isFeatherReleased = false;
            GameObject featherB = ObjectPoolManager.instance.featherPool.Get();
            featherB.transform.position = transform.position;

            // 레이캐스트로 적 감지 후 데미지 주기
            RaycastHit[] hitInfos = Physics.RaycastAll(transform.position + Vector3.up * 0.5f, dirB, featherDist, targetLayer); //dirFrAllyToEnm
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
                DamageParticle(hitinfo.transform.position + Vector3.up);


            }

            // 깃털 목적지로 옮기기
            float dist = 0;
            float nextDist = dist - 1;
            Vector3 destinationB;
            while (dist > nextDist)
            {
                destinationB = (transform.position + featherDist * dirB.normalized); // dirFrAllyToEnmNor
                dist = Vector3.Distance(featherB.transform.position, destinationB);
                featherB.transform.position += dirB.normalized * featherSpeedB * Time.deltaTime; // dirFrAllyToEnmNor
                nextDist = Vector3.Distance(featherB.transform.position, destinationB);
                yield return null;
            }

            if (featherB != null) StartCoroutine(ReleaseFeather(featherB, featherTime));


            i++;
            //yield return new WaitForSecondsRealtime(featherEftTime);

        }

        isBAttack = false;
        anim.SetTrigger("BLEND_TREE");
    }



    // 기본무기 강화 (연인의 도탄)
    private IEnumerator EvolveCrt()
    {
        isFeatherReleased = false;
        GameObject feather = ObjectPoolManager.instance.featherPool.Get();
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
                if (feather != null) pool.Release(feather.gameObject);
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
                        if (feather != null) pool.Release(feather.gameObject);
                        yield break;
                    }


                    yield return null;


                }

                enemies[i].collider.gameObject.GetComponent<EnemyHp>().UpdateHp(attackDmg);
                DamageParticle(enemies[i].collider.transform.position + Vector3.up);

            }

            if (i == 3)
            {
                if (feather != null) pool.Release(feather.gameObject);
                hp.Heal(hp.maxHealth * 0.1f * i);
            }
        }
        //yield return null;
    }



    // 고유 패시브 (전투 박쥐 교전)
    Vector3 p1;
    Vector3 p2;
    Vector3 p3;
    Vector3 p4;
    Vector3 p5;
    Vector3 p6;
    Vector3 p7;
    Vector3 p8;
    Vector3 dirP;

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

                isFeatherReleased = false;
                GameObject feather = ObjectPoolManager.instance.featherPool.Get();
                feather.transform.position = transform.position;
                feather.layer = LayerMask.NameToLayer("PassiveFeather");
                isFeatherReleased = false;
                GameObject feather2 = ObjectPoolManager.instance.featherPool.Get();
                feather2.transform.position = transform.position;
                feather2.layer = LayerMask.NameToLayer("PassiveFeather");

                dirP = p4 - transform.position;

                if(i == 0)
                {
                    // 몸통 회전시키기
                    allyBody.transform.forward = dirP;
                    //Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
                    ///////////////////////////////////////////
                    //transform.rotation = rotation;
                }

                while (true)
                {

                    dirP = p4 - transform.position;

                    time += Time.deltaTime * 3;

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
                        if (feather != null) pool.Release(feather.gameObject);
                        if (feather != null) pool.Release(feather2.gameObject);
                        break;
                    }
                    yield return null;

                    
                }

                yield return null;

            }

            if(targetP != null && targetP.GetComponent<EnemyMove>() != null)
            {
                targetP.GetComponent<EnemyHp>().UpdateHp(attackDmg * batRate * basicAttackNo);
                DamageParticle(targetP.transform.position + Vector3.up);
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


    // E 스킬용
    private IEnumerator StopEnemy(RaycastHit hitinfo)
    {
        hitinfo.transform.GetComponent<NavMeshAgent>().enabled = false;
        yield return new WaitForSecondsRealtime(enmStopTime);
        if(hitinfo.transform != null) hitinfo.transform.GetComponent<NavMeshAgent>().enabled = true;
    }

    private IEnumerator MoveFeather(Collider feather, Vector3 dir, Vector3 destination, float destroyTime)
    {
        // 깃털 모으기
        float dist = 0;
        float nextDist = dist - 1;
        while (dist > nextDist)
        {
            if(feather ==null)
            {
                yield break;
            }

            
            dist = Vector3.Distance(feather.transform.position, destination);
            feather.transform.position += dir * featherSpeedE * Time.deltaTime;
            nextDist = Vector3.Distance(feather.transform.position, destination);

            yield return null;
        }
        if(feather != null) StartCoroutine(ReleaseFeather(feather.gameObject, destroyTime));
    }

    

    // R 스킬용
    private IEnumerator Unbeatable()
    {
        unbeatable = true;
        allyNavMesh.moveSpeed *= 2;
        yield return new WaitForSecondsRealtime(2f);
        unbeatable = false;
        allyNavMesh.moveSpeed /= 2;
    }

    

    private IEnumerator ReleaseFeather(GameObject feather, float featherTime)
    {
        yield return new WaitForSecondsRealtime(featherTime);
        if (!isFeatherReleased && feather != null)
        {
            pool.Release(feather);
            isFeatherReleased = true;
        }
    }
    

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p4);
        
    }



}
