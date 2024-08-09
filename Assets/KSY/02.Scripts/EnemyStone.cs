using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStone : MonoBehaviour
{
    [Header("조절 가능")]
    [Tooltip("피격시 빨개지는 이펙트 지속 시간")]
    public float effTime;
    public float attackPower;
    public float attackRange;
    public float attackCoolTime;
    public float myMaxRange;
    public float indiSpawnCoolTime;


    List<GameObject> indiList = new List<GameObject>();

    [Header("터치 금지")]
    public GameObject indicatorPrf;
    GameObject indicator;
    int minIndiCnt = 10;
    float currTime;
    EnemyHp myHp;
    WaitForSeconds effTimeSec;
    Material myMaterial;
    Color orgColor;
    void Start()
    {
        myHp = GetComponent<EnemyHp>();
        myHp.onDie = OnDie;
        myHp.damageEff = DamageEff;
        myMaterial = GetComponentInChildren<MeshRenderer>().material;
        effTimeSec = new WaitForSeconds(effTime);
        orgColor = myMaterial.color;

        for(int i = 0; i < minIndiCnt; i++)
        {
            indicator = Instantiate(indicatorPrf);
            indiList.Add(indicator);
            indicator.SetActive(false);
        }
    }

    void Update()
    {
        currTime += Time.deltaTime;
        if(currTime > indiSpawnCoolTime)
        {
            Vector2 rand = Random.insideUnitCircle * myMaxRange;
            if(indiList.Count == 0)
            {
                indicator = Instantiate(indicatorPrf);

            }
            else
            {
                indicator = indiList[0];
                indiList.RemoveAt(0);
            }
            indicator.SetActive(true);
            indicator.transform.position = transform.position + Vector3.right * rand.x + Vector3.forward * rand.y;
        }
    }

    void DamageEff()
    {
        StartCoroutine(C_DamageEff());
    }

    void OnDie()
    {
        // 사라지고 Boss 다시 활동
    }

    IEnumerator C_DamageEff()
    {
        myMaterial.color = Color.red;
        yield return effTimeSec;
        myMaterial.color = orgColor;
    }
}
