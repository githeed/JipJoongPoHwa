using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorPref : MonoBehaviour
{
    public GameObject indicatorsBG;
    public GameObject indicator;

    public float attackPower;
    public float attackRange;
    public float attackCoolTime;

    

    private void OnEnable()
    {
        indicatorsBG.transform.localScale = Vector3.one * (attackRange +1);
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
