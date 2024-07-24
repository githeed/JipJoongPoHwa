using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_HPSystem : MonoBehaviour
{
    public float maxHealth;
    public float currHealth;
    public float timeTillReborn;
    bool isDead = false;


    private void Awake()
    {
        maxHealth = 100;
        currHealth = maxHealth;
        isDead = false;

    }

    // Start is called before the first frame update
    void Start()
    {
        timeTillReborn = 5f;
    }

    // Update is called once per frame
    void Update()
    {


    }

    public void Damaged(int damage)
    {
        if(isDead) return;
        

        currHealth -= damage;

        if (currHealth <= 0 && isDead == false)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        Destroy(gameObject);
    }

    public void Reborn()
    {
        StartCoroutine(RebornCrt());
    }

    public IEnumerator RebornCrt()
    {
        yield return new WaitForSecondsRealtime(timeTillReborn);
        isDead = false;

        Instantiate(gameObject);
    }
}
