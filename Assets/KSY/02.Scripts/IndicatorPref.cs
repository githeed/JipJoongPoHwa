using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class IndicatorPref : MonoBehaviour
{
    public GameObject arrowIndiBG;
    public GameObject arrowIndi;
    public GameObject circleIndi;

    public LayerMask players;
    float size = 1;
    public float attackPower;
    public float attackRange; // Player 피격 범위
    public float attackCoolTime;
    public float maxScale; // 콜라이더에 비해 빈공간이 있어서 attackRange +1;
    public bool isDestroy = false;

    public EnemyStone enemyStone;
    Material attackIndiMat;
    public bool isArrow;
    float duration;
    private void OnEnable()
    {
        circleIndi.SetActive(false);
        arrowIndi.SetActive(false);
        arrowIndiBG.SetActive(false);
        attackIndiMat = circleIndi.GetComponent<MeshRenderer>().material;
        arrowIndi.transform.localScale = Vector3.one;
        size = 1;
    }

    void Update()
    {
        if (isArrow)
        {
            if (!arrowIndi.activeSelf) arrowIndi.SetActive(true);
            if (!arrowIndiBG.activeSelf) arrowIndiBG.SetActive(true);
            if (circleIndi.activeSelf) circleIndi.SetActive(false);
            arrowIndiBG.transform.localScale = Vector3.one * (attackRange);
            maxScale = attackRange + 1;
            size += maxScale / attackCoolTime * Time.deltaTime;
            arrowIndi.transform.localScale = size * Vector3.one;
            if (size >= maxScale)
            {
                AttackCheck();
            }
        }
        else if(!isArrow)
        {
            if (arrowIndi.activeSelf) arrowIndi.SetActive(false);
            if (arrowIndiBG.activeSelf) arrowIndiBG.SetActive(false);
            if (!circleIndi.activeSelf) circleIndi.SetActive(true);
            circleIndi.transform.localScale = Vector3.one * (attackRange);
            duration += Time.deltaTime / attackCoolTime;
            attackIndiMat.SetFloat("_Duration", duration);
            if (duration >= 1)
            {
                AttackCheck();
            }
        }
        
    }


    void AttackCheck()
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
