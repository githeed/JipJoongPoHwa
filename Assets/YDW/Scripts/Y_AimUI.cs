using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_AimUI : MonoBehaviour
{
    Y_PlayerAttack yp;

    Transform nearestTarget;
    Vector3 dirToTarget;

    // Start is called before the first frame update
    void Start()
    {
        yp = GetComponentInParent<Y_PlayerAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        nearestTarget = yp.GetNearest();
        dirToTarget = nearestTarget.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(-transform.forward, dirToTarget);
    }
}
