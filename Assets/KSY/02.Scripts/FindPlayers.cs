using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindPlayers : MonoBehaviour
{
    public GameObject player0;
    public GameObject player1;
    public GameObject target;
    protected float dist0;
    protected float dist1;

    private void Awake()
    {
        player0 = GameObject.FindWithTag("Player");
        player1 = GameObject.FindWithTag("Player1");
    }
    void Update()
    {
        dist0 = (player0.transform.position - transform.position).magnitude;
        if (player1 != null && player1.activeSelf)
        {
            dist1 = (player1.transform.position - transform.position).magnitude;
            if (dist0 < dist1)
            {
                target = player0;
            }
            else
            {
                target = player1;
            }
        }
        if (player1 == null) target = player0;
        if (player1 != null && !player1.activeSelf && player0 != null) target = player0;
    }
}
