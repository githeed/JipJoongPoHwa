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
    Vector3 orgAttackIndiPos;
    Vector3 orgAttackIndiScale;
    Vector3 maxAttackIndiPos;
    Vector3 maxAttackIndiScale;
    public GameObject attackGroundEff;
    private void Awake()
    {
        orgAttackIndiPos = attackIndi.transform.localPosition;
        orgAttackIndiScale = attackIndi.transform.localScale;
        maxAttackIndiPos = BG.transform.localPosition;
        maxAttackIndiScale = BG.transform.localScale;
    }

    private void OnEnable()
    {
        attackIndi.transform.localScale = orgAttackIndiScale;
        attackIndi.transform.localPosition = orgAttackIndiPos;
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
        attackIndi.transform.localScale += (maxAttackIndiScale - orgAttackIndiScale) * (0.9f * Time.deltaTime / attackDelay);
        attackIndi.transform.localPosition += (maxAttackIndiPos - orgAttackIndiPos) * (0.9f * Time.deltaTime / attackDelay);
        if (currTime > attackDelay)
        {
            currTime = 0;
            attackGroundEff.SetActive(true);
            CheckOverlap();
        }
    }

}
