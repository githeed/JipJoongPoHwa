using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class IndicatorPref : MonoBehaviour
{
    public GameObject indicatorsBG;
    public GameObject indicator;

    public LayerMask players;
    float size = 1;
    public float attackPower;
    public float attackRange;
    public float attackCoolTime;
    public float maxScale; // 콜라이더에 비해 빈공간이 있어서 attackRange +1;


    private void OnEnable()
    {
        indicatorsBG.transform.localScale = Vector3.one * (attackRange +1);
        transform.localScale = Vector3.one;
        size = 1;
        maxScale = attackRange + 1;
    }

    
    void Update()
    {
        size += maxScale / attackCoolTime * Time.deltaTime;
        transform.localScale = size * Vector3.one;
        if (size >= maxScale)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange / 2f, players);
            foreach (var c in colliders)
            {
                if (c.gameObject.tag == "Player")
                {
                    c.GetComponent<H_PlayerAttack>().UpdateHp(attackPower);
                    print("indicator로 H 플레이어 맞음");
                }
                else if (c.gameObject.tag == "Player1")
                {
                    c.GetComponent<Y_PlayerAttack>().UpdateHp(attackPower);
                    print("indicator로 Y 플레이어 맞음");
                }
            }
            gameObject.SetActive(false);
        }
    }
}
