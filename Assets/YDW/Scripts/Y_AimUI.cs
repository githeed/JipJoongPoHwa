using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_AimUI : MonoBehaviour
{
    Y_PlayerAttack pa;
    GameObject ally;

    //GameObject nearestTarget;
    //Vector3 dirToTarget;

    // Start is called before the first frame update
    void Start()
    {
        ally = GameObject.Find("Ally");
        pa = ally.GetComponent<Y_PlayerAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pa.nearestTargetB != null) // && !pa.isBAttack && !pa.isESkill && !pa.isRSkill
        {
            // dirToTarget = nearestTarget.transform.position - transform.position;
            //dirToTarget = pa.dirB;
            transform.rotation = Quaternion.LookRotation(-transform.forward, pa.dirB);

        }
    }
}
