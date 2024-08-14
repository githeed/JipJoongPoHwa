using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAttackCanvas : MonoBehaviour
{
    
    float size = 1;
    public LayerMask players;
    public float attackPower;
    public float attackRange;
    public float attackCoolTime;
    public float maxScale; // 콜라이더에 비해 빈공간이 있어서 attackRange +1; // eff변경후 안해도 됨.
    GameObject BG;
    GameObject attackImage;
    Material attackMat;
    float duration;

    public GameObject attackEff;

    void OnEnable()
    {
        size = 1;
        duration = 0;
        maxScale = attackRange;
        BG = transform.GetChild(0).gameObject;
        attackImage = transform.GetChild(1).gameObject;
        attackMat = attackImage.GetComponent<MeshRenderer>().material;
        attackMat.SetFloat("_Duration", 0);
        transform.localPosition = Vector3.forward * attackRange /2 +Vector3.up*0.5f;
        BG.transform.localScale = Vector3.one * maxScale ;
        attackEff.transform.localScale = Vector3.one * 0.14f * maxScale;
        attackEff.SetActive(false);
    }

    void Update()
    {
        attackImage.transform.localScale = attackRange * Vector3.one;
        duration += Time.deltaTime / attackCoolTime;
        attackMat.SetFloat("_Duration", duration);

        if(IsDelayComplete(attackCoolTime - 1))
        {
            attackEff.SetActive(true);
        }

        if(duration >= 1)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange/2f, players);
            foreach(var c in colliders)
            {
                if(c.gameObject.tag == "Player")
                {
                    c.GetComponent<H_PlayerAttack>().UpdateHp(attackPower);
                }
                else if(c.gameObject.tag == "Player1")
                {
                    c.GetComponent<Y_PlayerAttack>().UpdateHp(attackPower);
                }
            }
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
