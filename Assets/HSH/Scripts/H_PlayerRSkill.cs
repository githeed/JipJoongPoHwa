using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_PlayerRSkill : MonoBehaviour
{

    public float moveSpeed = 20.0f;
    public float rRange = 5.0f;
    Vector3 startPos;

    GameObject player;
    H_PlayerAttack hm;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        hm = player.GetComponent<H_PlayerAttack>();
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        RSkillMove();
    }

    void RSkillMove()
    {
        hm.dirToR.y = 0;
        if (hm.rMag > rRange)
        {
            hm.rMag = rRange;
        }
        hm.endPos = transform.position;
        if ((hm.endPos - startPos).magnitude > hm.rMag)
        {
            hm.endPos = transform.position;
            hm.rMove = true;
            Destroy(gameObject);
        }
        transform.Translate(hm.dirToR * moveSpeed * Time.deltaTime);
    }
}
