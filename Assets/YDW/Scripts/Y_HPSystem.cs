using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_HPSystem : MonoBehaviour
{
    public float maxHealth;
    public float currHealth;
    public float timeTillReborn;
    public bool isDead = false;

    public GameObject allyBodyFactory;


    private void Awake()
    {
        maxHealth = 100;
        currHealth = maxHealth;
        isDead = false;

    }

    // Start is called before the first frame update
    void Start()
    {
        timeTillReborn = 2f;
    }

    // Update is called once per frame
    void Update()
    {


    }

    public void Damaged(float damage)
    {
        if(isDead) return;
        

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
            Destroy(GameObject.Find("AllyBody"));
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    public void Reborn()
    {
        StartCoroutine(RebornCrt());
    }

    private IEnumerator RebornCrt()
    {
        yield return new WaitForSecondsRealtime(timeTillReborn);
        isDead = false;

        if(this.name == "Ally")
        {
            var newPlayer = GameObject.Instantiate(allyBodyFactory);
            newPlayer.transform.parent = GameObject.Find("Ally").transform;
        }
        else
        {
            Instantiate(gameObject);
        }

        yield return null;

        // 이거 바로 Move 로 넘어갈라나? start 때문에?
    }
}
