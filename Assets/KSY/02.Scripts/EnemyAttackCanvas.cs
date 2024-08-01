using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCanvas : MonoBehaviour
{
    Canvas myCanvas;
    float size = 1;
    public LayerMask players;
    public float attackPower;

    void Start()
    {
        myCanvas = GetComponent<Canvas>();
        transform.localScale = Vector3.one;
    }

    void Update()
    {
        size += 2 * Time.deltaTime;
        transform.localScale = size * Vector3.one;
        if (size >= 5)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 2.5f, players);
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
            Destroy(gameObject);
            print("5가 되고 터짐!");
        }
    }
}
