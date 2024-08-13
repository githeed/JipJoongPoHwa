using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using Unity.Burst.Intrinsics;
using Unity.Entities.Hybrid.Baking;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class H_PlayerAttack : MonoBehaviour
{
    public List<GameObject> cutes = new List<GameObject>();
    

    Animator anim;

    private float curAttTime = 0;

    // 가까운 위치의 적 찾기
    public float scanRange = 10f;
    public LayerMask targetLayer;
    public Collider[] targets;
    public Transform nearestTarget;

    // 이펙트 공장
    public GameObject basicAttackEffFac;
    public GameObject hitEffecFac;

    // 잘가요..내사랑..
    //public GameObject scratchFac;
    public GameObject eBuff;
    public GameObject rEffectFac;
    public GameObject rReady;

    public float attackDmg = 5f;

    // 귀여운발사기
    public GameObject cuteRocket;


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

    public float rRange = 20;

    public Material mat;

    public float drainPower = 1;

    public float healPower = 10;

    public GameObject aim;

    UnityEngine.Rendering.Universal.UniversalAdditionalCameraData uac;

    // Start is called before the first frame update
    void Start()
    {
        print("Attack");
        H_PlayerManager.instance.curHP = H_PlayerManager.instance.maxHP;
        boxSize = new Vector3(H_PlayerManager.instance.xBox, 1, H_PlayerManager.instance.xBox);
        //mat = model.GetComponent<MeshRenderer>().GetComponent<Material>();
        anim = GetComponentInChildren<Animator>();

        for(int i = 0; i < 20; i++)
        {
            GameObject cr = Instantiate(cuteRocket);
            cr.SetActive(false);
            cutes.Add(cr);
        }

        uac = Camera.main.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
    }

    public float curA = 0;
    // Update is called once per frame
    void Update()
    {
        BasicAttack();
        if (Input.GetKeyDown(KeyCode.E) && !H_PlayerManager.instance.eCool)
        {
            GameManager.instance.eCoolText.enabled = true;
            H_PlayerManager.instance.eCool = true;
            BrierESkill();
        }
        BrierRSkill();
        RMove();
        if (H_PlayerManager.instance.isCuteOn)
        {
            CuteRocketAttack();
        }

        if (canE)
        {
            curA += Time.deltaTime;
            //if(curA <= 1)
            {
                //H_PlayerManager.instance.ChangeAlpha(Mathf.Lerp(0, 1, curA));
                //H_PlayerManager.instance.ChangeAlpha(0.8f);
                curA = 0;
            }

            currETime += Time.deltaTime;
            //|| dirToTarget == Vector3.zero
            if (currETime > ESkillTime)
            {
                canE = false;
                uac.renderPostProcessing = false;
                H_PlayerManager.instance.attTime = H_PlayerManager.instance.curAttDelay;
                currETime = 0;
                eBuff.GetComponent<ParticleSystem>().Stop();
                UpdateHp(-1 * healPower);
            }
        }
        else
        {
            //H_PlayerManager.instance.ChangeAlpha(0);
        }

    }

    public Transform GetNearest()
    {
        Transform result = null;
        float dist = 9999f;

        //Stopwatch stopwatch = new Stopwatch();
        //stopwatch.Start();
        // 오버랩 스피어 로 가까운 적을 찾자
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
        // 5초마다
        curAttTime += Time.deltaTime;
        if (curAttTime > H_PlayerManager.instance.attTime)
        {
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

                aim.transform.rotation = Quaternion.LookRotation(-aim.transform.forward, dirToTarget);

                // 공격범위를 정하자
                Vector3 boxPos = transform.position + dirToTarget * H_PlayerManager.instance.boxDist;

                // 공격범위를 기준으로 적만 맞는 박스콜라이더를 생성하자
                Collider[] enemies = Physics.OverlapBox(boxPos, boxSize * 0.5f, Quaternion.LookRotation(dirToTarget, transform.up), targetLayer);

                //Vector3 crossVec = Vector3.Cross(dirToTarget, transform.up);

                GameObject ef = Instantiate(basicAttackEffFac);
                ef.transform.forward = dirToTarget;
                ef.transform.eulerAngles += new Vector3(-90, 0, 0);
                ef.transform.position = transform.position + dirToTarget * H_PlayerManager.instance.boxDist;
                ef.transform.localScale = Vector3.one * H_PlayerManager.instance.effScale;

                //new Vector3(H_PlayerManager.instance.effScale, H_PlayerManager.instance.effScale, H_PlayerManager.instance.effScale);

                Destroy(ef, 0.4f);
                // 잘가..요 내사랑..

                // 이펙트 생성
                //GameObject ef = Instantiate(scratchFac);
                //GameObject ef1 = Instantiate(scratchFac);
                //crossVec.Normalize();

                //// 앞방향의 양옆으로 이펙트의 위치를 정해주자
                //ef.transform.localScale = new Vector3(H_PlayerManager.instance.effScale, H_PlayerManager.instance.effScale, H_PlayerManager.instance.effScale);
                //ef1.transform.localScale = new Vector3(H_PlayerManager.instance.effScale, H_PlayerManager.instance.effScale, H_PlayerManager.instance.effScale);
                //ef.transform.position = boxPos + -1 * crossVec;
                //ef.transform.rotation = Quaternion.LookRotation(-Vector3.up, dirToTarget);
                //ef1.transform.position = boxPos + 1 * crossVec;
                //ef1.transform.rotation = Quaternion.LookRotation(Vector3.up, dirToTarget);


                //// 0.4 초후에 이펙트를 없애자
                //Destroy(ef, 0.4f);
                //Destroy(ef1, 0.4f);

                curAttTime = 0;

                // 범위에 들어온 적들의 피를 깎자
                foreach (Collider enemy in enemies)
                {

                    EnemyHp em = enemy.GetComponent<EnemyHp>();
                    if (em != null)
                    {
                        em.UpdateHp(attackDmg);

                        // 타격 이펙트 자리
                        GameObject he = Instantiate(hitEffecFac);
                        he.transform.localScale = Vector3.one * 5;
                        he.transform.position = enemy.gameObject.transform.position + Vector3.up * 3.0f;
                        Destroy(he, 0.4f);
                        if(canE)
                        {
                            UpdateHp(-1 * drainPower);
                        }
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
            //mat.color = new Color(0, 1, 0);
            // e 를 사용하면 기본공격의 쿨타임을 줄이자
            canE = true;
            H_PlayerManager.instance.attTime = 0.2f;
            
            uac.renderPostProcessing = true;
            eBuff.GetComponent<ParticleSystem>().Play();
        }
        else
        {
            // e를 사용중이라면 돌아가자
            canE = false;
            H_PlayerManager.instance.attTime = H_PlayerManager.instance.curAttDelay;
            currETime = 0;
            eBuff.GetComponent<ParticleSystem>().Stop();
            UpdateHp(-1 * healPower);
        }
    }
    void BrierRSkill()
    {
        // R 눌렀을때 오브젝트 마우스 포인터 방향으로 던지기
        if (Input.GetKeyDown(KeyCode.R) && !H_PlayerManager.instance.rCool)
        {
            rReady.GetComponent<ParticleSystem>().Play();
            Coroutine cr = StartCoroutine(PushRSkill());
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
        GameObject re = Instantiate(rEffectFac);
        re.transform.position = transform.position;
        Destroy(re, 2);

        Collider[] cols = Physics.OverlapSphere(transform.position, rRange, targetLayer);
        foreach (Collider col in cols)
        {
            //print(col + " " + rDamage);
            col.GetComponent<EnemyHp>().UpdateHp(rDamage);
        }
        BrierESkill();
    }

    void CuteRocketAttack()
    {
        H_PlayerManager.instance.cuteCurAttDelay += Time.deltaTime;

        if (H_PlayerManager.instance.cuteCurAttDelay > H_PlayerManager.instance.cuteAttTime)
        {
            GameObject cr = GetCute();
            cr.transform.position = transform.position;

            H_PlayerManager.instance.cuteCurAttDelay = 0;
        }
    }

    public void UpdateHp(float dmg)
    {
        H_PlayerManager.instance.curHP -= dmg;
        Y_DamageUI yd = GetComponentInChildren<Y_DamageUI>();

        if (H_PlayerManager.instance.curHP >= H_PlayerManager.instance.maxHP)
        {
            H_PlayerManager.instance.curHP = H_PlayerManager.instance.maxHP;
        }

        StartCoroutine(yd.ChangeColorTemporarily());
        if (H_PlayerManager.instance.curHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawCube(transform.position + dirToTarget * boxDist, boxSize);
    //}

    GameObject GetCute()
    {
        GameObject cute = null;
        if (cutes.Count > 0)
        {
            cute = cutes[0];
            cute.SetActive(true);
            cutes.RemoveAt(0);
        }
        else
        {
            cute = Instantiate(cuteRocket);
        }

        return cute;
    }

    IEnumerator PushRSkill()
    {
        yield return new WaitForSeconds(0.5f);

        rReady.GetComponent<ParticleSystem>().Stop();

        GameManager.instance.rCoolText.enabled = true;
        H_PlayerManager.instance.rCool = true;
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
