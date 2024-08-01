﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using Unity.Entities.Hybrid.Baking;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

public class H_PlayerAttack : MonoBehaviour
{
    //public List<GameObject> enemies = new List<GameObject>();

    Animator anim;


    public float attTime = 5;
    float currAttDelay = 0;
    private float curAttTime = 0;

    // 가까운 위치의 적 찾기
    public float scanRange = 10f;
    public LayerMask targetLayer;
    public Collider[] targets;
    public Transform nearestTarget;

    // 이펙트 공장
    public GameObject scratchFac;

    public float attackDmg = 5f;

    public float maxHP = 1000;
    float curHP = 0;


    // 박스 의 방향 벡터
    public Vector3 dirToTarget;

    //박스의 사이즈
    Vector3 boxSize;


    // E스킬 사용 가능여부
    public bool canE = false;
    public bool canR = false;

    // E 스킬 지속시간
    public float ESkillTime = 5.0f;
    float currETime = 0;

    // R 스킬 방향, 단위
    public Vector3 dirToR;
    public float rMag = 0;

    public GameObject rSkillObj;

    public float rMoveTime = 2;

    GameObject obj;

    Vector3 stPos;
    public Vector3 endPos;
    public bool rMove = false;

    //public GameObject model;

    public int rDamage = 10;

    public Material mat;

    // Start is called before the first frame update
    void Start()
    {
        print("Attack");
        curHP = maxHP;
        boxSize = new Vector3(H_PlayerManager.instance.xBox, 1, H_PlayerManager.instance.xBox);
        //mat = model.GetComponent<MeshRenderer>().GetComponent<Material>();
        currAttDelay = attTime;

        anim = GetComponentInChildren<Animator>();

    }

    public float curA = 0;
    // Update is called once per frame
    void Update()
    {
        BasicAttack();
        if (Input.GetKeyDown(KeyCode.E))
        {
            BrierESkill();
        }
        BrierRSkill();
        RMove();

        if(canE)
        {
            curA += Time.deltaTime;
            //if(curA <= 1)
            {
                //H_PlayerManager.instance.ChangeAlpha(Mathf.Lerp(0, 1, curA));
                H_PlayerManager.instance.ChangeAlpha(0.8f);
                curA = 0;
            }
        }
        else
        {
            H_PlayerManager.instance.ChangeAlpha(0);
        }


        currETime += Time.deltaTime;
        if (currETime > ESkillTime || dirToTarget == Vector3.zero)
        {
            canE = false;
            attTime = 1;
            currETime = 0;

        }
    }

    Transform GetNearest()
    {
        Transform result = null;
        float dist = 9999f;

        //Stopwatch stopwatch = new Stopwatch();
        //stopwatch.Start();


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
        if (curAttTime > attTime)
        {
            // 오버랩 스피어 로 가까운 적을 찾자
            targets = Physics.OverlapSphere(transform.position, scanRange, targetLayer);
            nearestTarget = GetNearest();

            if (nearestTarget == null) return;
            // 공격하기
            else
            {
                anim.SetTrigger("ATTACK");

                // 타겟의 방향을 가져오자
                dirToTarget = (nearestTarget.position - transform.position);
                dirToTarget.y = 0;
                dirToTarget.Normalize();

                // 공격범위를 정하자
                Vector3 boxPos = transform.position + dirToTarget * H_PlayerManager.instance.boxDist;

                // 공격범위를 기준으로 적만 맞는 박스콜라이더를 생성하자
                Collider[] enemies = Physics.OverlapBox(boxPos, boxSize * 0.5f, Quaternion.LookRotation(dirToTarget, transform.up), targetLayer);

                Vector3 crossVec = Vector3.Cross(dirToTarget, transform.up);

                // 이펙트 생성
                GameObject ef = Instantiate(scratchFac);
                GameObject ef1 = Instantiate(scratchFac);
                crossVec.Normalize();
                // 앞방향의 양옆으로 이펙트의 위치를 정해주자
                ef.transform.localScale = new Vector3(H_PlayerManager.instance.effScale, H_PlayerManager.instance.effScale, H_PlayerManager.instance.effScale);
                ef1.transform.localScale = new Vector3(H_PlayerManager.instance.effScale, H_PlayerManager.instance.effScale, H_PlayerManager.instance.effScale);
                ef.transform.position = boxPos + -1 * crossVec;
                ef.transform.rotation = Quaternion.LookRotation(-Vector3.up, dirToTarget);
                ef1.transform.position = boxPos + 1 * crossVec;
                ef1.transform.rotation = Quaternion.LookRotation(Vector3.up, dirToTarget);


                // 0.4 초후에 이펙트를 없애자
                Destroy(ef, 0.4f);
                Destroy(ef1, 0.4f);

                curAttTime = 0;

                // 범위에 들어온 적들의 피를 깎자
                foreach (Collider enemy in enemies)
                {

                    EnemyHp em = enemy.GetComponent<EnemyHp>();
                    if (em != null)
                    {
                        em.UpdateHp(attackDmg);
                    }
                    //print(enemy.gameObject + ": " + attackDmg);
                }

                //nearestTarget.gameObject.GetComponent<EnemyMove>().UpdateHp(attackDmg);
                //ObjectPoolManager.instance
            }

        }
    }

    void BrierESkill()
    {
        // 광란스킬
        if (!canE)
        {
            // e 를 사용하면 기본공격의 쿨타임을 줄이자
            canE = true;
            attTime = 0.2f;
        }
        else
        {

            // e를 사용중이라면 돌아가자
            canE = false;
            attTime = currAttDelay;
            currETime = 0;
        }
    }
    void BrierRSkill()
    {
        // R 눌렀을때 오브젝트 마우스 포인터 방향으로 던지기
        if (Input.GetKeyDown(KeyCode.R))
        {
            obj = Instantiate(rSkillObj);
            stPos = transform.position;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                dirToR = hit.point - transform.position;
                dirToR.y = 0;
                rMag = dirToR.magnitude;
                dirToR.Normalize();
            }
            else
            {
                dirToR = transform.forward * 10;
            }
            obj.transform.position = transform.position;
        }
    }

    void RMove()
    {
        // R스킬 오브젝트 위치로 이동
        if (rMove)
        {
            canR = true;
            Vector3 dir = endPos - transform.position;
            transform.Translate(dir * 20 * Time.deltaTime);
            float mag = (transform.position - stPos).magnitude;
            if (mag > rMag - 0.3f)
            {
                rMove = false;
                canR = false;
                RBlow();
            }
        }
    }

    void RBlow()
    {
        // R 스킬 터질때
        Collider[] cols = Physics.OverlapSphere(transform.position, 10, targetLayer);
        foreach (Collider col in cols)
        {
            //print(col + " " + rDamage);
            col.GetComponent<EnemyHp>().UpdateHp(rDamage);
        }
        BrierESkill();
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

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawCube(transform.position + dirToTarget * boxDist, boxSize);
    //}
}
