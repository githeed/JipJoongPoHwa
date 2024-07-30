using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Nokturne : MonoBehaviour
{
    GameObject target;
    public GameObject attackIndicatorPos;
    Vector3 dir;
    Vector3 forwardDir;
    public float attackDelay;
    WaitForSeconds attackDelays;
    Ray toTarget;
    RaycastHit hitInfo;
    bool canAttack;
    bool attacking;
    public float attackSpeed;
    Vector3 destination;
    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        attackIndicatorPos.SetActive(false);
        attackDelays = new WaitForSeconds(attackDelay);
        canAttack = true;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        target = GetComponent<EnemyMove>().target;
        dir = target.transform.position - transform.position;
        toTarget = new Ray(transform.position, dir);
        if(Physics.Raycast(toTarget, out hitInfo, 5, 1 << LayerMask.NameToLayer("Player")))
        {
            StartCoroutine(NocturneAttack());
        }
        if (attacking) 
        {
            transform.Translate(dir.normalized * attackSpeed * Time.deltaTime);
            if()
        }




    }

    IEnumerator NocturneAttack()
    {
        if (canAttack)
        {
            canAttack = false;
            agent.enabled = false;
            destination = target.transform.position;
            attackIndicatorPos.SetActive(true);
            attackIndicatorPos.transform.forward = new Vector3(dir.normalized.x, 0, dir.normalized.z);
            yield return attackDelays;

        }
        canAttack = true;

    }
}
