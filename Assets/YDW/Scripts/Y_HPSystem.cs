using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_HPSystem : MonoBehaviour
{
    public float maxHealth;
    public float currHealth;
    public float timeTillReborn;
    public bool isDead;
    public bool rebornable;

    public GameObject allyBody;
    public GameObject ally;
    public GameObject playerBody;
    Y_AllyFSM allyFSM;


    private void Awake()
    {
        maxHealth = 1000;
        currHealth = maxHealth;
        isDead = false;
        rebornable = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        timeTillReborn = 2f;
        allyBody = GameObject.Find("AllyBody");
        ally = GameObject.Find("Ally");
        playerBody = GameObject.Find("Player");
        allyFSM = allyBody.GetComponentInParent<Y_AllyFSM>();
    }

    // Update is called once per frame
    void Update()
    {


    }

    public void Damaged(float damage)
    {
        if(isDead) return;

        currHealth -= damage;
        //allyFSM.hasDamaged = true;
        allyFSM.HitAlly(damage);

        if (currHealth <= 0 && isDead == false)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        if(this.name == "Ally")
        {
            GameObject.Find("AllyBody").SetActive(false);
        }
        else
        {
            playerBody.SetActive(false);
        }
        
    }

    public void Reborn()
    {
        if(rebornable)
        {
            StartCoroutine(RebornCrt());
            rebornable = false;
        }

    }

    private IEnumerator RebornCrt()
    {
        
        yield return new WaitForSecondsRealtime(timeTillReborn);
        

        if(this.name == "Ally")
        {
            ally.transform.position = playerBody.transform.position + 3 * transform.right;
            allyBody.SetActive(true);
        }
        else
        {
            playerBody.SetActive(true);
        }

        currHealth = maxHealth;
        isDead = false;
        rebornable = true;
        allyFSM.playChooseDir();

    }
}
