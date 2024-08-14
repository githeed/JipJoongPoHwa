using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_PlayerMove : MonoBehaviour
{
    [SerializeField]
    float pMoveSpeed = 7.0f;
    public GameObject model;
    [SerializeField]
    CharacterController cc;

    Animator anim;

    H_PlayerAttack pa;

    Vector3 dir;
    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        pa = GetComponent<H_PlayerAttack>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("MOVE", dir.magnitude);
        Player_Move();
    }

    void Player_Move()
    {
        Player_E_Move();
        if (H_PlayerManager.instance.canE || pa.canR) return;
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        dir = new Vector3(h, 0, v);
        dir.Normalize();

        cc.Move(dir * pMoveSpeed * Time.deltaTime);

        if (dir != Vector3.zero)
        {
            model.transform.forward = dir;
        }
    }

    void Player_E_Move()
    {
        if (H_PlayerManager.instance.canE)
        {
            dir = pa.dirToTarget;
            model.transform.forward = dir;
            float mag = (pa.GetNearest().position - transform.position).magnitude;
            //print(mag);
            if(mag > 1)
            {
                cc.Move(dir * pMoveSpeed * Time.deltaTime);
            }    
        }
    }
}
