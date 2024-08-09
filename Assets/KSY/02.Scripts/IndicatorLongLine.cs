using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorLongLine : MonoBehaviour
{
    BoxCollider myCollider;
    GameObject player0;
    GameObject player1;
    Collider collider0;
    Collider collider1;
    H_PlayerAttack H_Player;
    Y_PlayerAttack Y_Player;
    public float attackPower;
    // Start is called before the first frame update
    void Start()
    {
        myCollider = GetComponent<BoxCollider>();
        player0 = GameObject.FindWithTag("Player");
        player1 = GameObject.FindWithTag("Player1");
        collider0 = player0.GetComponent<Collider>();
        collider1 = player1.GetComponent<Collider>();
        H_Player = player0.GetComponent<H_PlayerAttack>();
        Y_Player = player1.GetComponent<Y_PlayerAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckOverlap()
    {
        if (myCollider.bounds.Intersects(collider0.bounds))
        {
            H_Player.UpdateHp(attackPower);
        }
        if (myCollider.bounds.Intersects(collider1.bounds))
        {

        }
    }

    public void AttackPlayer()
    {

    }
}
