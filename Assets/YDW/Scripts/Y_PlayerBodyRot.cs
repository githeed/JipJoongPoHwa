using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_PlayerBodyRot : MonoBehaviour
{
    Y_AllyFSM allyFSM;
    GameObject allyBody;

    // Start is called before the first frame update
    void Start()
    {
        allyFSM = GetComponent<Y_AllyFSM>();
        allyBody = GameObject.Find("AllyBody");
    }

    // Update is called once per frame
    void Update()
    {
        allyBody.transform.forward = allyFSM.moveDir;
    }
}
