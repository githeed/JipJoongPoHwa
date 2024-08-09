using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_ExpBall : MonoBehaviour
{

    GameObject player;
    float alpha;
    float dist;
    Vector3 dir;
    public float collectRange = 10;
    public float dieRange = 0.4f;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        dir = player.transform.position - transform.position;
        dir.Normalize();
        alpha = Time.deltaTime;
        dist = Vector3.Distance(player.transform.position, transform.position);
        if(dist < collectRange)
        {
            transform.position = Vector3.Lerp(transform.position, player.transform.position, alpha * 10);
        }
        if(dist < dieRange)
        {
            H_PlayerManager.instance.UpdateExp(1);
            gameObject.SetActive(false);
            H_PlayerManager.instance.exps.Add(gameObject);
        }
    }
}
