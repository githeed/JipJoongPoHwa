using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_PlayerAttack : MonoBehaviour
{
    public List<GameObject> enemies = new List<GameObject>();

    public float attTime = 5;
    private float curAttTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        BasicWeaponAttack();
    }

    GameObject FindClosestEnemy()
    {
        float dist = 9999f;
        GameObject closestEnemy = null;
        if(enemies.Count != 0)
        {
            for(int i = 0; i < enemies.Count; i++)
            {
                if (Vector3.Distance(transform.position, enemies[i].transform.position) < dist)
                {
                    dist = Vector3.Distance(transform.position, enemies[i].transform.position);
                    closestEnemy = enemies[i];
                }
            }
        }
        return closestEnemy;
    }

    void BasicWeaponAttack()
    {
        curAttTime += Time.deltaTime;
        if (curAttTime > attTime)
        {
            if (FindClosestEnemy() != null)
            {
                print(FindClosestEnemy());
            }
            else
            {
                print("null");
            }
            curAttTime = 0;
        }
    }
}
