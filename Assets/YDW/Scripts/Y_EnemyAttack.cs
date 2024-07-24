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
        allyFSM.HitPlayer(attackPower);
    }
}
