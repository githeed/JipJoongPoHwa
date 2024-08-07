using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_Cute : MonoBehaviour
{
    public GameObject player;
    H_PlayerAttack pa;
    Transform target;

    Vector3 dir;

    public float cuteSpeed = 30;
    public float cuteDmg = 1;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        pa = player.GetComponent<H_PlayerAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        target = pa.GetNearest();
        dir = target.position - transform.position;
        dir.Normalize();
        transform.Translate(dir * cuteSpeed * Time.deltaTime);

        float dist = Vector3.Distance(target.position, transform.position);
        if(dist < 0.3f)
        {
            EnemyHp eh = target.GetComponent<EnemyHp>();
            eh.UpdateHp(cuteDmg);
            print(cuteDmg);
            gameObject.SetActive(false);
            pa.cutes.Add(gameObject);
        }
    }

    
}
