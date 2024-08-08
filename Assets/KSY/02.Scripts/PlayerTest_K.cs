using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest_K : MonoBehaviour
{
    public float maxHP;
    public float curHP;

    // Start is called before the first frame update
    void Start()
    {
        curHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHP(float dmg)
    {
        curHP -= dmg;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            other.GetComponent<EnemyHp>().UpdateHp(10);
        }
    }
}
