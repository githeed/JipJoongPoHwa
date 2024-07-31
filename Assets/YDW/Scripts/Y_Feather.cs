using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_Feather : MonoBehaviour
{
    Y_PlayerAttack yp;
    public bool isGround = false;

    // Start is called before the first frame update
    void Start()
    {
        yp = GameObject.Find("Ally").GetComponent<Y_PlayerAttack>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //private void OnTriggerEnter(Collider other)
    //{
    //    if(other.gameObject.layer == LayerMask.NameToLayer("Ground"))
    //    {
    //        isGround = true;
    //    }

    //    if(other.gameObject.layer == LayerMask.NameToLayer("Enemy") && !isGround)
    //    {
    //        print("!!!!!!!!!!!!!!!!");
    //        other.gameObject.GetComponent<EnemyMove>().UpdateHp(yp.attackDmg * yp.batRate);
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if(other.gameObject.layer == LayerMask.NameToLayer("Ground"))
    //    {
    //        isGround = false;
    //    }
    //}
}
