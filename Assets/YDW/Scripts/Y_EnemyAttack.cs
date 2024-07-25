using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_EnemyAttack : MonoBehaviour
{
    public float attackPower;
    public Y_AllyFSM allyFSM;

    // Start is called before the first frame update
    void Start()
    {
        attackPower = 50;
        allyFSM = GameObject.Find("Ally").GetComponent<Y_AllyFSM>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!allyFSM.hasDamaged)
        {
            allyFSM.hasDamaged = true;
            allyFSM.HitAlly(attackPower);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        allyFSM.hasDamaged = false;
    }
}
