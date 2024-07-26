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

    public GameObject scratchFac;

    public float  attackDmg = 5f;

    public float maxHP = 1000;
    float curHP = 0;
    Vector3 boxSize;
    public Vector3 dirToTarget;
    float xBox = 10f;
    float zBox = 10f;
    public float boxDist = 1f;

    public bool canE = false;

    public float ESkillTime = 5.0f;
    float currETime = 0;

    // Start is called before the first frame update
    void Start()
    {
        curHP = maxHP;
        boxSize = new Vector3(xBox, 1, zBox);
    }

    // Update is called once per frame
    void Update()
    {
        BasicAttack();
        BrierESkill();



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
                dirToTarget = (nearestTarget.position - transform.position);
                dirToTarget.y = 0;
                dirToTarget.Normalize();
                Vector3 boxPos = transform.position + dirToTarget * boxDist;
                Collider[] enemies = Physics.OverlapBox(boxPos, boxSize * 0.5f);
                
                Vector3 crossVec = Vector3.Cross(dirToTarget, transform.up);

                GameObject ef = Instantiate(scratchFac);
                GameObject ef1 = Instantiate(scratchFac);
                crossVec.Normalize();
                ef.transform.position = boxPos + -1 * crossVec;
                ef.transform.rotation = Quaternion.LookRotation(-Vector3.up, dirToTarget);
                ef1.transform.position = boxPos + 1 * crossVec;
                ef1.transform.rotation = Quaternion.LookRotation(Vector3.up, dirToTarget);


                Destroy(ef, 0.4f);
                Destroy(ef1, 0.4f);

                curAttTime = 0;
                
                foreach (Collider enemy in enemies)
                {
                    
                     EnemyMove em = enemy.GetComponent<EnemyMove>();
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
        if(Input.GetKeyDown(KeyCode.E))
        {
            print("PressE");
            if (!canE)
            {
                print("CantE");

                canE = true;
                attTime = 0.2f;
            }
            else
            {
                print("CanE");

                canE = false;
                attTime = 1;
                currETime = 0;
            }
        }
        //if(Input.GetKeyDown(KeyCode.E) && !canE)
        //{
        //    canE = true;
        //    attTime = 0.2f;
        //}

        //if(Input.GetKeyDown(KeyCode.E) && canE)
        //{
        //    canE = false;
        //    attTime = 1;
        //}
    }

    public void UpdateHp(float dmg)
    {
        curHP -= dmg;
        //print(curHP);
        if (curHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position + dirToTarget * boxDist, boxSize);
    }
}
