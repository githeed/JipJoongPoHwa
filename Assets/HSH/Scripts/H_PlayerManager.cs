﻿using System.Collections;
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


    public float[] maxExperiences = new float[16];
    public int indexLev = 0;
    public int expMultiplier = 5;

    public float exp = 1;

    public float maxExp = 0;

    public bool bIsPicking = false;

    // 박스의 x, z 사이즈
    public float xBox = 10f;
    //float zBox = 10f;

    // 박스의 전방위치
    public float boxDist = 1f;

    public float effScale = 0.5f;

    public float skillRCooltime = 50.0f;
    public float skillECooltime = 10.0f;

    Coroutine pc;

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
        ChangeAlpha(0);
    }

    void Update()
    {

        if(bIsPicking) 
        {
            Time.timeScale = 0;
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
        xBox ++;
        boxDist += 0.5f;
        effScale += 0.5f;
        bIsPicking = false;
        print("endpick");
        weaponBtn.enabled = false;
        img.enabled = false;
        bg.enabled = false;

    }

    public void CardPickingButton()
    {
        StopCoroutine(pc);
        //print(effScale);
        print("pick");

        xBox++;
        boxDist += 0.5f;
        effScale += 0.5f;
        Time.timeScale = 1;
        bIsPicking = false;
        weaponBtn.enabled = false;
        img.enabled = false;
        bg.enabled = false;

    }

    float currTime;
    public bool SkillCoolTime(float time)
    {
        currTime += Time.deltaTime;
        if (currTime > time)
        { 
            currTime = 0;
            return true;
        }
        return false;
    }

    public void ChangeAlpha(float alpha)
    {
        e_UI.color = new Color(1, 0, 0, alpha);
        
    }
}
