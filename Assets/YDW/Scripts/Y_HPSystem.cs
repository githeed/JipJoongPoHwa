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

    GameObject allyBody;
    GameObject playerBody;
    Y_AllyFSM allyFSM;


    private void Awake()
    {
        maxHealth = 100;
        currHealth = maxHealth;
        isDead = false;
        rebornable = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        timeTillReborn = 2f;
        allyBody = GameObject.Find("AllyBody");
        playerBody = GameObject.Find("Player_Y_copied");
        allyFSM = allyBody.GetComponentInParent<Y_AllyFSM>();
    }

    // Update is called once per frame
    void Update()
    {


    }

    public void Damaged(float damage)
    {
        Debug.Log("damaged return 전");
        if(isDead) return;
        Debug.Log("damaged return 후");

        currHealth -= damage;
        print(currHealth);

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
            allyBody.SetActive(true);
            print("부활했다!!");
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
