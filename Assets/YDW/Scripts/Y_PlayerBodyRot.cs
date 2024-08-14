using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_PlayerBodyRot : MonoBehaviour
{
    Y_AllyFSM allyFSM;
    GameObject allyBody;
    Y_PlayerAttack pa;
    Y_NavMesh yn;

    // Start is called before the first frame update
    void Start()
    {
        allyFSM = GetComponent<Y_AllyFSM>();
        allyBody = GameObject.Find("AllyBody");
        pa = GetComponent<Y_PlayerAttack>();
        yn = GetComponent<Y_NavMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!pa.isBAttack && !pa.isESkill && !pa.isRSkill)
            allyBody.transform.forward = yn.agent.destination - transform.position;
    }
}
