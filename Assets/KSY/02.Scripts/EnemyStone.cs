using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStone : MonoBehaviour
{
    EnemyHp myHp;
    public float effTime;
    WaitForSeconds effTimeSec;
    Material myMaterial;
    Color orgColor;
    void Start()
    {
        myHp = GetComponent<EnemyHp>();
        myHp.onDie = OnDie;
        myHp.damageEff = DamageEff;
        myMaterial = GetComponentInChildren<MeshRenderer>().material;
        effTimeSec = new WaitForSeconds(effTime);
        orgColor = myMaterial.color;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            DamageEff();
        }
    }

    void DamageEff()
    {
        StartCoroutine(C_DamageEff());
    }

    void OnDie()
    {

    }

    IEnumerator C_DamageEff()
    {
        myMaterial.color = Color.red;
        yield return effTimeSec;
        myMaterial.color = orgColor;
    }
}
