using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Y_HPSystem : MonoBehaviour
{
    public float maxHealth = 500;
    public float currHealth;
    public float timeTillReborn;
    public bool isDead;
    public bool rebornable;

    public GameObject allyBody;
    public GameObject ally;
    public GameObject playerBody;
    Y_AllyFSM allyFSM;
    Y_DamageUI yd;

    public UnityEngine.UI.Image hpBar;
    public GameObject aimUI;
    public GameObject hpUI;





    private void Awake()
    {
        currHealth = maxHealth;
        isDead = false;
        rebornable = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        timeTillReborn = 7f;
        allyBody = GameObject.Find("AllyBody");
        ally = GameObject.Find("Ally");
        playerBody = GameObject.Find("Player");
        allyFSM = allyBody.GetComponentInParent<Y_AllyFSM>();
        yd = allyBody.GetComponentInChildren<Y_DamageUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isStop) return;
        UpdateHP();

    }

    public void UpdateHP()
    {
        hpBar.fillAmount = currHealth / maxHealth;
    }

    public void Damaged(float damage)
    {
        if(isDead) return;

        currHealth -= damage;
        //allyFSM.hasDamaged = true;
        allyFSM.HitAlly(damage);

        StartCoroutine(yd.ChangeColorTemporarily());
        //StartCoroutine(yd.RedVignette());

        if (currHealth <= 0 && isDead == false)
        {
            Die();
        }
    }

    public void Heal(float amt)
    {
        if (isDead) return;

        currHealth += amt;
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        hpUI.SetActive(false);

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
        
        yield return new WaitForSeconds(timeTillReborn);
        

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
        aimUI.SetActive(true);
        hpUI.SetActive(true);
        rebornable = true;
        allyFSM.playChooseDir();

    }
}
