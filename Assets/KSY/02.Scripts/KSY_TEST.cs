using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSY_TEST : MonoBehaviour
{
    
    public GameObject bossprf;
    public GameObject midBossprf;
    public GameObject nocturne;

    public bool SpawnNocturne;
    public bool SpawnMidBoss;
    public bool SpawnBoss;

    GameObject go;
    public Transform spawnPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SpawnBoss)
        {
            SpawnBoss = false;
            go = Instantiate(bossprf);
            go.transform.position = spawnPos.position;
        }
        if (SpawnMidBoss)
        {
            SpawnMidBoss = false;
            go = Instantiate(midBossprf);
            go.transform.position = spawnPos.position;
        }
        if (SpawnNocturne)
        {
            SpawnNocturne = false;
            go = Instantiate(nocturne);
            go.transform.position = spawnPos.position;
        }
    }
}
