﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public Image hpBar;
    public Image expBar;
    public Text hpText;
    public Text lvText;
    public Image eCool;
    public Image rCool;
    public Text eCoolText;
    public Text rCoolText;


    public GameObject player;

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
        
    }

    void Update()
    {
        SetHPText();
        SetHPBar();
        SetLVText();
        SetExpText();
        SetECool();
        SetRCool();
    }

    void SetHPBar()
    {
        hpBar.fillAmount = H_PlayerManager.instance.curHP / H_PlayerManager.instance.maxHP;
    }

    void SetHPText()
    {
        hpText.text = H_PlayerManager.instance.curHP.ToString() + " / " + H_PlayerManager.instance.maxHP.ToString();
    }

    void SetLVText()
    {
        lvText.text = H_PlayerManager.instance.indexLev.ToString();
    }

    void SetExpText()
    {
        expBar.fillAmount = H_PlayerManager.instance.exp / H_PlayerManager.instance.maxExperiences[H_PlayerManager.instance.indexLev];
    }

    void SetECool()
    {
        if(H_PlayerManager.instance.eCool)
        {
            eCool.fillAmount = 1 - H_PlayerManager.instance.curECoolTime / H_PlayerManager.instance.skillECooltime;
        }
        else
        {
            eCool.fillAmount = 0;
        }
    }

    void SetRCool()
    {
        if (H_PlayerManager.instance.rCool)
        {
            rCool.fillAmount = 1 - H_PlayerManager.instance.curRCoolTime / H_PlayerManager.instance.skillRCooltime;
        }
        else
        {
            rCool.fillAmount = 0;
        }
    }

    public void SetECoolText()
    {
        int ec = (int)H_PlayerManager.instance.skillECooltime - (int)H_PlayerManager.instance.curECoolTime;
        eCoolText.text = ec.ToString();
    }

    public void SetRCoolText()
    {
        int rc = (int)H_PlayerManager.instance.skillRCooltime - (int)H_PlayerManager.instance.curRCoolTime;
        rCoolText.text = rc.ToString();
    }
}
