using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public float attackDelay;
    float currTime;
    public GameObject BG;
    public GameObject attackIndi;



    private void OnEnable()
    {
        attackIndi.transform.localScale = Vector3.one - Vector3.right * 0.9f;
    }

    void Start()
    {
        myCollider = GetComponent<BoxCollider>();
        player0 = GameObject.FindWithTag("Player");
        player1 = GameObject.FindWithTag("Player1");
        collider0 = player0.GetComponent<Collider>();
        collider1 = player1.GetComponent<Collider>();
        H_Player = player0.GetComponent<H_PlayerAttack>();
        Y_Player = player1.GetComponent<Y_PlayerAttack>();

        gameObject.SetActive(false);
    }

    
    // Update is called once per frame
    void Update()
    {
        AttackPlayer();
    }

    public void CheckOverlap()
    {
        if (myCollider.bounds.Intersects(collider0.bounds))
        {
            H_Player.UpdateHp(attackPower);
            print("H플레이어 때림");
        }
        if (myCollider.bounds.Intersects(collider1.bounds))
        {
            Y_Player.UpdateHp(attackPower);
            print("Y플레이어 때림");
        }
        gameObject.SetActive(false);
    }

    public void AttackPlayer()
    {
        currTime += Time.deltaTime;
        attackIndi.transform.localScale += Vector3.right * (0.9f * Time.deltaTime / attackDelay);
        if (currTime > attackDelay)
        {
            currTime = 0;
            CheckOverlap();
        }
    }
}
