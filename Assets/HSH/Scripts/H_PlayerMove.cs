using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_PlayerMove : MonoBehaviour
{
    [SerializeField]
    float pMoveSpeed = 7.0f;
    [SerializeField]
    GameObject model;
    [SerializeField]
    CharacterController cc;

    H_PlayerAttack pa;
    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        pa = GetComponent<H_PlayerAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        Player_Move();
    }

    void Player_Move()
    {
        Player_E_Move();
        if (pa.canE) return;
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h, 0, v);
        dir.Normalize();

        cc.Move(dir * pMoveSpeed * Time.deltaTime);

        if (dir != Vector3.zero)
        {
            model.transform.forward = dir;
        }
    }

    void Player_E_Move()
    {
        if (pa.canE)
        {
            Vector3 dir = pa.dirToTarget;
            model.transform.forward = dir;
            dir.Normalize();
            cc.Move(dir * pMoveSpeed * Time.deltaTime);
        }
    }
}
