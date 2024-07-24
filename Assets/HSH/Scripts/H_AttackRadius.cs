using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_AttackRadius : MonoBehaviour
{
    H_PlayerAttack pa; 
    // Start is called before the first frame update
    void Start()
    {
        pa = GetComponentInParent<H_PlayerAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name.Contains("enem"))
        {
            pa.enemies.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Contains("enem"))
        {
            pa.enemies.Remove(other.gameObject);
        }
    }

}
