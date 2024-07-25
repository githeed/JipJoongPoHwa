using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_EnemyMove_copied : MonoBehaviour
{
    [SerializeField]
    float pMoveSpeed = 7.0f;
    [SerializeField]
    GameObject model;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h, 0, v);
        dir.Normalize();

        transform.position += dir * pMoveSpeed * Time.deltaTime;
        if (dir != Vector3.zero)
        {
            model.transform.forward = dir;
        }
    }
}
