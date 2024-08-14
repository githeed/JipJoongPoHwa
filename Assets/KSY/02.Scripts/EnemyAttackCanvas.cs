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
    }

    void Update()
    {
        attackImage.transform.localScale = attackRange * Vector3.one;
        duration += Time.deltaTime / attackCoolTime;
        attackMat.SetFloat("_Duration", duration);

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
}
