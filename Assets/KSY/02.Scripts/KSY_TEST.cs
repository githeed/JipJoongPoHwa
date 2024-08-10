using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSY_TEST : MonoBehaviour
{
    public Transform bossSpawnPos;
    public Transform bossMoveTarget;
    public Transform cameraMoveTarget;
    public GameObject bossprf;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            GameObject boss = Instantiate(bossprf);
            boss.transform.position = bossSpawnPos.position;
            Boss bossCs = boss.GetComponent<Boss>();
            bossCs.bossMoveTarget = bossMoveTarget;
            bossCs.mainCamTargetPos = cameraMoveTarget;
        }
    }
}
