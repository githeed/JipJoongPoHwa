using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorPrefOfStone : MonoBehaviour
{
    public GameObject indicator;

    public LayerMask players;
    float size = 1;
    public float attackPower;
    public float attackRange; // Player 피격 범위
    public float attackCoolTime;
    public float maxScale; // 콜라이더에 비해 빈공간이 있어서 attackRange +1;
    public bool isDestroy = false;

    public EnemyStone enemyStone;
    Material attackIndiMat;
    float duration;
    public GameObject attackEff;

    private void OnEnable()
    {
        indicator.transform.localScale = Vector3.one*attackRange;
        attackIndiMat = indicator.GetComponent<MeshRenderer>().material;
        //attackIndiMat.SetFloat("_Duration", 0);
        attackEff.SetActive(false);
        duration = 0;
    }

    void Update()
    {
        maxScale = attackRange + 1;
        duration += Time.deltaTime / attackCoolTime;
        attackIndiMat.SetFloat("_Duration", duration);
        if(duration>=0.9f) attackEff.SetActive(true); 
        if (duration >= 1)
        {
            
            Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange / 2f, players);
            foreach (var c in colliders)
            {
                if (c.gameObject.tag == "Player")
                {
                    c.GetComponent<H_PlayerAttack>().UpdateHp(attackPower);
                }
                else if (c.gameObject.tag == "Player1")
                {
                    c.GetComponent<Y_PlayerAttack>().UpdateHp(attackPower);
                }
            }
            if (isDestroy) Destroy(gameObject);
            if (enemyStone != null) enemyStone.indiList.Add(gameObject);
            gameObject.SetActive(false);
        }
    }

    float currTime;
    bool IsDelayComplete(float delayTime) // 딜레이 시간
    {
        // 시간을 증가 시키자.
        currTime += Time.deltaTime;
        // 만약에 시간이 delayTime보다 커지면
        if (currTime >= delayTime)
        {
            // 현재시간 초기화
            currTime = 0;
            // true 반환
            return true;
        }
        // 그렇지 않으면
        else
        {
            // false 반환
            return false;
        }
    }
}
