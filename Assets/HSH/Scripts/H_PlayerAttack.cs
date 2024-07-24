using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_PlayerAttack : MonoBehaviour
{
    //public List<GameObject> enemies = new List<GameObject>();

    public float attTime = 5;
    private float curAttTime = 0;

    public float scanRange = 5f;
    public LayerMask targetLayer;
    public Collider[] targets;
    public Transform nearestTarget;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        targets = Physics.OverlapSphere(transform.position, scanRange, targetLayer);
    }

    Transform GetNearest()
    {
        Transform result = null;
        float dist = 9999f;

        foreach(Collider target in targets)
        {
            float curDist = Vector3.Distance(transform.position, target.transform.position);
            if(curDist < dist)
            {
                dist = curDist;
                result = target.transform;
            }
        }
        return result;
    }

    void BasicAttack()
    {
        nearestTarget = GetNearest();

        curAttTime += Time.deltaTime;

        if(curAttTime > attTime)
        {

            curAttTime = 0;
        }
    }
}
