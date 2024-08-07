using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using UnityEngine;

public class EnemyHp : MonoBehaviour
{

    public float maxHp;
    public float curHp;

    public Action onDie;
    public Action<float> onDamageUI;
    void OnEnable()
    {
        curHp = maxHp;
    }

    /// <summary>
    /// 매개변수에 float형으로 공격력(attackPower)넣어주면 됨.
    /// </summary>
    /// <param name="dmg"></param>
    public void UpdateHp(float dmg)
    {
        if (curHp <= 0) return;
        if(onDamageUI != null) onDamageUI(dmg);
        curHp -= dmg;
        if (curHp <= 0)
        {
            if(onDie != null)onDie();
        }
    }
}
