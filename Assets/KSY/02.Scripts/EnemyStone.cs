using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStone : MonoBehaviour
{
    EnemyHp myHp;
    void Start()
    {
        myHp = GetComponent<EnemyHp>();
        myHp.onDie = OnDie;
        myHp.damageEff = DamageEff;
    }

    void Update()
    {
        
    }

    void DamageEff()
    {

    }

    void OnDie()
    {

    }
}
