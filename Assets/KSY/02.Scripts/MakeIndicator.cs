using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeIndicator : MonoBehaviour
{
    public Canvas indicatorFac;
    public Canvas indicator;
    public float attackPower;

    void Start()
    {
        
    }

    void Update()
    {
        if (!Input.GetKey(KeyCode.LeftControl)) return;
        if (Input.GetMouseButtonDown(0))
        {
            indicator = Instantiate(indicatorFac);
            indicator.GetComponentInChildren<EnemyAttackCanvas>().attackPower = attackPower;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if(Physics.Raycast(ray, out hitInfo))
            {
                indicator.transform.position = hitInfo.point + Vector3.up;
            }
        }
    }

    public void MakeAttackIndicator(float attackPower)
    {
        indicator = Instantiate(indicatorFac);
        indicator.GetComponentInChildren<EnemyAttackCanvas>().attackPower = attackPower;
    }

}
