using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCanvas : MonoBehaviour
{
    
    float size = 1;
    public LayerMask players;
    public float attackPower;
    public float attackRange;
    public float attackCoolTime;
    public float maxScale; // 콜라이더에 비해 빈공간이 있어서 attackRange +1;
    GameObject BG;
    GameObject attackImage;

    void OnEnable()
    {
        size = 1;
        maxScale = attackRange + 1;
        BG = transform.GetChild(0).gameObject;
        attackImage = transform.GetChild(1).gameObject;
        transform.localPosition = Vector3.forward * attackRange /2 +Vector3.up*0.5f;
        BG.transform.localScale = Vector3.one * maxScale ;
    }

    void Update()
    {
        size += maxScale / attackCoolTime * Time.deltaTime;
        attackImage.transform.localScale = size * Vector3.one;
        if (size >= maxScale)
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
}
