﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;



public class Spawner : MonoBehaviour
{
    Transform player;
    public float coolTime;
    float currTime;
    public float distanceMin;
    public float distanceMax;
    Vector2 distance;
    float rand;
    float randDirX;
    float randDirZ;
    Vector3 dir;
    public Transform leftTopTr;
    public Transform rightBottomTr;
    Vector3 leftTop;
    Vector3 rightBottom;
    

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        distance = new Vector2(distanceMin, distanceMax);
        leftTop = leftTopTr.position;
        rightBottom = rightBottomTr.position;
    }

    // Update is called once per frame
    void Update()
    {
        currTime += Time.deltaTime;
        
        if (currTime >= coolTime)
        {
            currTime = 0;
            rand = Random.Range(distanceMin, distanceMax);
            randDirX = Random.Range(-1f, 1f);
            randDirZ = Random.Range(-1f, 1f);
            while (randDirX == 0 && randDirZ == 0)
            {
                randDirX = Random.Range(-1f, 1f);
                randDirZ = Random.Range(-1f, 1f);
            }
            dir = new Vector3(randDirX, 0, randDirZ);

            GameObject go = ObjectPoolManager.instance.pool.Get();
            go.GetComponent<EnemyMove>().OnNav();
            go.transform.position = player.position + (dir.normalized * rand) + Vector3.up*0.1f;
            if(go.transform.position.x < leftTop.x)
            {
                go.transform.position = new Vector3(leftTop.x, go.transform.position.y, go.transform.position.z);
            }
            if (go.transform.position.z > leftTop.z)
            {
                go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, leftTop.z);
            }
            if(go.transform.position.x > rightBottom.x)
            {
                go.transform.position = new Vector3(rightBottom.x, go.transform.position.y, go.transform.position.z);
            }
            if(go.transform.position.z < rightBottom.z)
            {
                go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, rightBottom.z);
            }

        }
    }
}
