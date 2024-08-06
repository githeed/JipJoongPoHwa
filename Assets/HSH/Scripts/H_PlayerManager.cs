using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class H_PlayerManager : MonoBehaviour
{
    public static H_PlayerManager instance;
    public GameObject cardButton;
    Button weaponBtn;
    Image img;
    public Image bg;
    public Image e_UI;

    public float attTime = 7;
    public float curAttDelay = 0;

    public float[] maxExperiences = new float[16];
    public int indexLev = 0;
    public int expMultiplier = 5;

    public float exp = 1;

    public float maxExp = 0;

    public bool bIsPicking = false;

    // 박스의 x, z 사이즈
    public float xBox = 10f;
    public float boxMultiplier = 1;
    //float zBox = 10f;

    // 박스의 전방위치
    public float boxDist = 1f;
    public float distMultiplier = 1;

    public float effScale = 0.5f;
    public float effMultiplier = 1;

    public bool eCool = false;
    public bool rCool = false;

    public float curECoolTime = 0;
    public float curRCoolTime = 0;
    public float skillRCooltime = 50.0f;
    public float skillECooltime = 10.0f;

    Coroutine pc;

    public float maxHP = 1000;
    public float curHP = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        // 그렇지 않다면
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        for (int i = 0; i < maxExperiences.Length; i++)
        {
            maxExperiences[i] = i * expMultiplier;
        }
        indexLev = 1;
        print("Manager");
        weaponBtn = cardButton.GetComponent<Button>();
        img = cardButton.GetComponent<Image>();
        //ChangeAlpha(0);

        curAttDelay = attTime;
    }

    void Update()
    {
        if(bIsPicking) 
        {
            Time.timeScale = 0;
        }

        if(eCool)
        {
            GameManager.instance.SetECoolText();

            curECoolTime += Time.deltaTime;
            if(curECoolTime > skillECooltime)
            {
                GameManager.instance.eCoolText.enabled = false;
                curECoolTime = 0;
                eCool = false;
            }
        }

        if (rCool)
        {
            GameManager.instance.SetRCoolText();
            curRCoolTime += Time.deltaTime;
            if (curRCoolTime > skillRCooltime)
            {
                GameManager.instance.rCoolText.enabled = false;
                curRCoolTime = 0;
                eCool = false;
            }
        }

    }

    public void UpdateExp(float value)
    {
        if (indexLev > maxExperiences.Length) return;
        exp += value;
        if (exp >= maxExperiences[indexLev])
        {
            weaponBtn.enabled = true;
            img.enabled = true;
            bg.enabled = true;
            indexLev++;
            bIsPicking = true;
            pc = StartCoroutine(PickCard(10));
            print("start pick");
        }
    }

    IEnumerator PickCard(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1;
        xBox += boxMultiplier;
        boxDist += distMultiplier;
        effScale += effMultiplier;
        bIsPicking = false;
        print("endpick");
        weaponBtn.enabled = false;
        img.enabled = false;
        bg.enabled = false;
        attTime = curAttDelay;
        attTime -= 0.3f;
        curAttDelay = attTime;

    }

    public void CardPickingButton()
    {
        StopCoroutine(pc);
        //print(effScale);
        xBox += boxMultiplier;
        boxDist += distMultiplier;
        effScale += effMultiplier;
        Time.timeScale = 1;
        bIsPicking = false;
        weaponBtn.enabled = false;
        img.enabled = false;
        bg.enabled = false;
        attTime = curAttDelay;
        attTime -= 0.3f;
        curAttDelay = attTime;
    }

    //public void ChangeAlpha(float alpha)
    //{
    //    e_UI.color = new Color(1, 0, 0, alpha);
        
    //}
}
