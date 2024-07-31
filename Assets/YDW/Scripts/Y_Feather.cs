using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_Feather : MonoBehaviour
{
    Y_PlayerAttack yp;
    // Start is called before the first frame update
    void Start()
    {
        yp = GameObject.Find("Ally").GetComponent<Y_PlayerAttack>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.GetComponent<EnemyMove>().UpdateHp(yp.attackDmg * yp.batRate);
    }
}
