using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Entities.UniversalDelegates;
using Unity.PlasticSCM.Editor.WebApi;
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
    public float basicAttTime = 4;
    public float ESkillTime;
    public float RSkillTime;
    public float PSkillDuration = 15;
    public float EvSkillTime = 10;
    public float EvSkillDuration;
    public float curBAttTime = 0;
    public float curEAttTime = 0;
    public float curRAttTime = 0;
    public float curPAttTime = 0;
    public float curEvAttTime = 0;
    public float skillTimeRate = 1;
    public float featherEftTime = 1f;
    public float particleEftTime = 1f;

    // Scan and Target
    public float scanRange = 50f;
    public LayerMask targetLayer;
    public LayerMask featherLayer;
    public Collider[] targets;
    //public Collider[] feathers;
    public GameObject[] feathersE;
    public GameObject[] feathersR;
    public Transform nearestTarget;

    // AttackDmg and HP
    public float attackDmg = 50;
    Y_HPSystem hp;

    // Feather Attack
    public GameObject featherFactory;
    public GameObject featherEFactory;
    public GameObject featherAFactory;
    public GameObject basicAttEffFactory;
    public float basicAttackNo = 3;
    public float featherDist = 14;
    public float featherTime = 10;


    public float eAttRate = 1.2f;
    public float enmStopTime = 3f;

    public float featherRNo = 24;

    public bool unbeatable = false;
    Y_AllyFSM AllyFSM;

    public bool pAttacking = false;
    public float batRate = 1.05f;

    public float featherSpeedB = 50;
    public float featherSpeedE = 100;
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

    public GameObject shield;


    public IObjectPool<GameObject> pool { get; set; }
    public IObjectPool<GameObject> particlePool { get; set; } 
    public IObjectPool<GameObject> dmgParticlePool { get; set; } 



    void Start()
    {
        hp = GetComponent<Y_HPSystem>();

        RSkillTime = 9999;
        ESkillTime = 9999;
       

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

        if (GameManager.instance.isStop) return;

        ////////////////////////////
        if (!hp.isDead)
        {

            curBAttTime += Time.deltaTime * skillTimeRate;
            curEAttTime += Time.deltaTime * skillTimeRate;
            curRAttTime += Time.deltaTime * skillTimeRate;
            curEvAttTime += Time.deltaTime * skillTimeRate;

            if (curBAttTime > basicAttTime) // && !isESkill && !isRSkill
            {
                BasicAttack();
                curBAttTime = 0;
            }

            else if (curEAttTime > ESkillTime) // && !isBAttack && !isRSkill
            {
                StartCoroutine(ESkill());
                curEAttTime = 0;
            }

            else if (curRAttTime > RSkillTime) // && !isBAttack && !isESkill
            {
                StartCoroutine(RSkill());
                curRAttTime = 0;
            }

            else if (curEvAttTime > EvSkillTime)
            {
                if (pm.indexLev >= 7)
                {
                    StartCoroutine(EvolveCrt());
                    curEvAttTime = 0;
                }
            }
        }

        

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            StartCoroutine(PassiveAttack());
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            StartCoroutine(RSkill());
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            StartCoroutine(ESkill());
        }








        // 레벨별로 속도나 개수 변경하기
        if (pm.indexLev == 1)
        {
            basicAttTime = 5;
            basicAttackNo = 3;

        }
        else if(pm.indexLev == 2)
        {
            basicAttTime = 4f;
        }
        else if (pm.indexLev == 3)
        {
            basicAttTime = 3f;
            ESkillTime = 9;
            basicAttackNo = 4;

        }
        else if (pm.indexLev == 4)
        {
            basicAttTime = 2f;
            ESkillTime = 7;
        }
        else
        {
            ESkillTime = 5;
            basicAttackNo = 5;
            RSkillTime = 20;
        }

        //if(pm.indexLev >= 5)
        //{
        //    curEvAttTime += Time.deltaTime;
        //    if(curEvAttTime > EvSkillTime)
        //    {
        //        StartCoroutine(EvolveCrt());
        //        curEvAttTime = 0;
        //    }
        //}


        batRate = 1.05f + 0.1f * pm.indexLev;


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
        // 오버랩 스피어wwww
        // targets = Physics.OverlapSphere(transform.position, scanRange, targetLayer);
        nearestTargetB = GetNearest().gameObject;

        dirB = nearestTargetB.transform.position - transform.position;
        if (nearestTargetB == null) return;
        // 공격하기
        else
        {
            StartCoroutine(FeatherAttack());
        }
        Quaternion rotation = Quaternion.LookRotation(dirB, Vector3.up);
        allyBody.transform.rotation = rotation;

        curBAttTime = 0;

    }




    private IEnumerator ESkill()
    {
        isESkill = true;
        anim.SetTrigger("ESKILL");
        yield return new WaitForSeconds(0.6f);

        Collider[] feathers = Physics.OverlapSphere(transform.position, 99999f, featherLayer);


       // StopAllCoroutines();

        foreach (Collider feather in feathers)
        {
            feather.gameObject.name = "feather";

            Vector3 dirFrFthToAlly = transform.position - feather.gameObject.transform.position;
            dirFrFthToAlly.y = 0;
            Vector3 dirFrFthToAllyNor = dirFrFthToAlly.normalized;

            AttackParticle(feather.gameObject, dirFrFthToAlly);
            SoundManager.instance.PlayZayahSound(1);

            Vector3 destinationE = transform.position;

            feather.transform.position = transform.position;

            if(feather != null)
            {
                feather.GetComponent<Y_Feather>().StopDestroy();
                feather.GetComponent<Y_Feather>().StartDestroy(featherTime); 

            }

            //StopCoroutine(ReleaseFeather(feather.gameObject, featherTime));
            //StartCoroutine(ReleaseFeather(feather.gameObject, 0.1f)); ///////////////

            //StartCoroutine(MoveFeather(feather, dirFrFthToAllyNor, destinationE, 0.1f));



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

        yield return new WaitForSeconds(0.8f);

        for (int i = 1; i <= featherRNo; i++)
        {

            // 깃털 360도로 퍼지게
            GameObject feather = ObjectPoolManager.instance.featherPool.Get();
            feather.layer = LayerMask.NameToLayer("Feather");
            //feather.transform.rotation = Quaternion.LookRotation(transform.forward, transform.up);
            feather.transform.localEulerAngles = new Vector3(0, (360 / featherRNo) * i, 0);
            feather.transform.position = transform.position + featherDist * feather.transform.forward;
            feather.name = "feather " + i;

            // 파티클 재생
            AttackParticle(gameObject, feather.transform.forward);

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
            if (feather != null)
            {

                feather.GetComponent<Y_Feather>().StopDestroy();
                feather.GetComponent<Y_Feather>().StartDestroy(featherTime);

                //StopCoroutine(ReleaseFeather(feather.gameObject, featherTime));
                //StartCoroutine(ReleaseFeather(feather, featherTime));
            }
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


    void AttackParticle(GameObject obj, Vector3 dir)
    {
        GameObject basicAttEff = ObjectPoolManager.instance.attackParticlePool.Get();
        //GameObject basicAttEff = Instantiate(basicAttEffFactory);
        basicAttEff.transform.position = transform.position;
        basicAttEff.transform.forward = dir;
        basicAttEff.transform.localScale = Vector3.one * 8.0f;
        basicAttEff.transform.position = obj.transform.position + 3 * Vector3.up;
        StartCoroutine(ReleaseAttParticle(basicAttEff, particleEftTime));
        //Destroy(basicAttEff, particleEftTime);
        //pool.Release(basicAttEff);
    }

    IEnumerator ReleaseAttParticle(GameObject basicAttEff, float time)
    {
        yield return new WaitForSeconds(time);
        particlePool.Release(basicAttEff);
    }

    public void RemoveFeather()
    {
        Collider[] feathers = Physics.OverlapSphere(transform.position, 99999f, featherLayer);

        foreach (Collider feather in feathers)
        {
            if(feather != null)
            {
                feather.gameObject.GetComponent<Y_Feather>().StopDestroy();
                feather.gameObject.GetComponent<Y_Feather>().StartDestroy(featherEftTime);

            }
            //StopCoroutine(ReleaseFeather(feather.gameObject, featherTime));
            //StartCoroutine(ReleaseFeather(feather.gameObject, featherTime));
        }
    }

    void DamageParticle(Vector3 dir)
    {
        //GameObject dp = Instantiate(damageParticle);
        GameObject dp = ObjectPoolManager.instance.damageParticlePool.Get();
        dp.transform.localScale = Vector3.one * 5.0f;
        dp.transform.position = dir;
        //dmgParticlePool.Release(dp);
        StartCoroutine(ReleaseDmgParticle(dp, 1f));
    }

    IEnumerator ReleaseDmgParticle(GameObject particle, float time)
    {
        yield return new WaitForSeconds(time);
        dmgParticlePool.Release(particle);
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
        yield return new WaitForSeconds(0.3f);

        //Quaternion rotation = Quaternion.LookRotation(dirFrAllyToEnm, Vector3.up);
        //transform.rotation = rotation;
        //allyBody.transform.forward = dirFrAllyToEnm;
        allyBody.transform.forward = dirB;

        yield return new WaitForSeconds(0.4f);


        while (i < basicAttackNo)
        {
            // 쏘아지는 이펙트 만들고 파괴
            AttackParticle(gameObject, dirB);
            SoundManager.instance.PlayZayahSound(0);

            // 레이캐스트로 적 감지 후 데미지 주기
            RaycastHit[] hitInfos = Physics.RaycastAll(transform.position + Vector3.up * 0.5f, dirB, featherDist, targetLayer); //dirFrAllyToEnm
            System.Array.Sort(hitInfos, (x, y) => x.distance.CompareTo(y.distance));
            int nth = 0;
            foreach (RaycastHit hitinfo in hitInfos)
            {
                // 1: 0%, 2: 15%, 3:30%, 4: 45%, 5:60%
                if (nth < 5)
                {
                    hitinfo.transform.GetComponent<EnemyHp>().UpdateHp(attackDmg * (1f - ((nth * 15f) / 100f)));
                }
                else
                {
                    hitinfo.transform.GetComponent<EnemyHp>().UpdateHp(attackDmg * 0.4f);
                }
                nth++;
                
                DamageParticle(hitinfo.transform.position + Vector3.up);


            }

            // 깃털 목적지에 넣음
            GameObject featherB = ObjectPoolManager.instance.featherPool.Get();
            featherB.transform.localEulerAngles = new Vector3(0, 0, 0);
            featherB.layer = LayerMask.NameToLayer("Feather");
            featherB.transform.position = transform.position + dirB.normalized * featherDist;

            //// 깃털 생성 후 Ally 위치에 놓음
            ////GameObject feather = Instantiate(featherFactory);
            //GameObject featherB = ObjectPoolManager.instance.featherPool.Get();
            //featherB.transform.localEulerAngles = new Vector3(0, 0, 0);
            //featherB.layer = LayerMask.NameToLayer("Feather");
            //featherB.transform.position = transform.position;

            //// 깃털 목적지로 옮기기
            //float dist = 0;
            //float nextDist = dist - 1;
            //Vector3 destinationB;
            //while (dist > nextDist)
            //{
            //    destinationB = (transform.position + featherDist * dirB.normalized); // dirFrAllyToEnmNor
            //    dist = Vector3.Distance(featherB.transform.position, destinationB);
            //    featherB.transform.position += dirB.normalized * featherSpeedB * Time.deltaTime; // dirFrAllyToEnmNor
            //    nextDist = Vector3.Distance(featherB.transform.position, destinationB);
            //    yield return null;
            //}

            //StopCoroutine(ReleaseFeather(featherB.gameObject, featherTime));
            //StartCoroutine(ReleaseFeather(featherB, featherTime));
            featherB.GetComponent<Y_Feather>().StopDestroy();
            featherB.GetComponent<Y_Feather>().StartDestroy(featherTime);


            i++;
            yield return new WaitForSecondsRealtime(featherEftTime);

        }

        isBAttack = false;
        anim.SetTrigger("BLEND_TREE");
    }




    // 기본무기 강화 (연인의 도탄)
    private IEnumerator EvolveCrt()
    {
        print("EvolveCrt 실행");
        // Initiate feather
        GameObject feather = Instantiate(featherEFactory);
        feather.layer = LayerMask.NameToLayer("Feather");
        feather.transform.position = transform.position;
        feather.transform.localEulerAngles = new Vector3(0, 0, 0);

        // 리스트 만들어줌
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

        // 튕기게 하기
        for (int i = 0; i < 4; i++)
        {
            if (i >= enemies.Count || enemies[i].collider == null)
            {
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
                        if (feather != null && feather.activeSelf) Destroy(feather);
                        yield break;
                    }


                    yield return null;


                }

                enemies[i].collider.gameObject.GetComponent<EnemyHp>().UpdateHp(attackDmg);
                DamageParticle(enemies[i].collider.transform.position + Vector3.up);

            }

            if (i == 3)
            {
                if (feather != null && feather.activeSelf) Destroy(feather);
                shield.SetActive(true);
                //GameObject shield = Instantiate(shield);
                shield.transform.position = transform.position;
                hp.Heal(hp.maxHealth * 0.1f * i);
                yield return new WaitForSeconds(2f);
                shield.SetActive(false);
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
            if (hp.isDead) yield break;

            //curPAttTime += Time.deltaTime;

            //p1 = transform.position;
            //p2 = transform.position + 5f * transform.right;// + transform.forward * -10f;
            //p3 = transform.position - 5f * transform.right;

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

                GameObject feather1 = Instantiate(featherAFactory);
                feather1.transform.position = transform.position;
                feather1.layer = LayerMask.NameToLayer("PassiveFeather");
                feather1.name = "feather";
                GameObject feather2 = Instantiate(featherAFactory);
                feather2.transform.position = transform.position;
                feather2.layer = LayerMask.NameToLayer("PassiveFeather");
                feather2.name = "feather";

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
                    if (hp.isDead) yield break;

                    p1 = transform.position;
                    p2 = transform.position + 5f * transform.right;// + transform.forward * -10f;
                    p3 = transform.position - 5f * transform.right;

                    dirP = p4 - transform.position;

                    time += Time.deltaTime * 3;

                    time = Mathf.Clamp(time, 0, 1);

                    p5 = Vector3.Lerp(p1, p2, time);
                    p6 = Vector3.Lerp(p2, p4, time);

                    p7 = Vector3.Lerp(p1, p3, time);
                    p8 = Vector3.Lerp(p3, p4, time);

                    feather1.transform.position = Vector3.Lerp(p5, p6, time);
                    feather2.transform.position = Vector3.Lerp(p7, p8, time);

                    if (time >= 1)
                    {
                        Destroy(feather1);
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
                DamageParticle(targetP.transform.position + Vector3.up);
            }
            yield return new WaitForSeconds(1f);
            curPAttTime += 1;
        }

        pAttacking = false;
    }

    private IEnumerator SkillTimeFast()
    {
        skillTimeRate = 1.1f;
        yield return new WaitForSeconds(15f);
        skillTimeRate = 1f;
    }


    // E 스킬용
    private IEnumerator StopEnemy(RaycastHit hitinfo)
    {
        hitinfo.transform.GetComponent<NavMeshAgent>().enabled = false;
        yield return new WaitForSeconds(enmStopTime);
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
        feather.gameObject.GetComponent<Y_Feather>().StopDestroy();
        feather.gameObject.GetComponent<Y_Feather>().StartDestroy(destroyTime);
        //StopCoroutine(ReleaseFeather(feather.gameObject, featherTime));
        //StartCoroutine(ReleaseFeather(feather.gameObject, destroyTime));
    }

    

    // R 스킬용
    private IEnumerator Unbeatable()
    {
        unbeatable = true;
        allyNavMesh.moveSpeed *= 2;
        yield return new WaitForSeconds(2f);
        unbeatable = false;
        allyNavMesh.moveSpeed /= 2;
    }

    

    //private IEnumerator ReleaseFeather(GameObject feather, float featherTime)
    //{
    //    yield return new WaitForSeconds(featherTime);
    //    if (feather != null && feather.activeSelf)
    //    {
    //        pool.Release(feather);
    //    }
    //}
    

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p4);
        
    }



}
