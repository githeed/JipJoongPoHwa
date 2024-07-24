using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    GameObject target;
    public float moveSpeed;
    Vector3 dir;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        dir = target.transform.position - transform.position;

        transform.Translate(dir.normalized * moveSpeed * Time.deltaTime);
    }
}
