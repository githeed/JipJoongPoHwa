using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public Text timeText;

    public float gameTime = 0;

    public GameObject player;

    public bool bISWin = false;

    public Transform bossSpawnPos;
    public Transform bossMoveTarget;
    public Transform cameraMoveTarget;
    public GameObject bossprf;
    public GameObject midBossPrf;
    public GameObject nocturnePrf;
    GameObject midBoss;
    GameObject spawner;

    [Header("조절 가능")]
    [Tooltip("게임 시작 후 보스 스폰 시간(분 단위로 입력)")]
    public float bossSpawnMin;
    [Tooltip("게임 시작 후 중간 보스 스폰 시간(초 단위로 입력)")]
    public float midBossSpawnPeriod;
    float midBossSpawnTimer;
    int midBossNum;
    bool bossSpawn;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // 씬 전환이 되도 게임오브젝트를 파괴하고 싶지않다.
            DontDestroyOnLoad(gameObject);
        }
        // 그렇지 않다면
        else
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        if (player == null) player = GameObject.FindWithTag("Player");
        if (spawner == null) spawner = GameObject.FindWithTag("Spawner");
    }

    void Update()
    {
        gameTime += Time.deltaTime;
        if (H_PlayerManager.instance.curHP <= 0)
        {
            bISWin = false;
            SceneManager.LoadScene("EndUIScene");
        }

        if(gameTime >= 60*bossSpawnMin & !bossSpawn)
        {
            bossSpawn = true;
            GameObject boss = Instantiate(bossprf);
            boss.transform.position = bossSpawnPos.position;
            Boss bossCs = boss.GetComponent<Boss>();
            bossCs.bossMoveTarget = bossMoveTarget;
            bossCs.mainCamTargetPos = cameraMoveTarget;

        }
        midBossSpawnTimer += Time.deltaTime;
        if(midBossSpawnTimer > midBossSpawnPeriod)
        {
            midBossSpawnTimer -= midBossSpawnPeriod;
            if(midBossNum % 2 == 0)
            {
                midBoss = Instantiate(midBossPrf);
            }
            else if(midBossNum % 2 == 1)
            {
                midBoss = Instantiate(nocturnePrf);
            }
            //midBoss.transform.position = player.transform.position;
            spawner.GetComponent<Spawner>().SetPos(midBoss);
            midBossNum++;
        }
        //if (gameTime > 15)
        //{
        //    bISWin = true;
        //    SceneManager.LoadScene("EndUIScene");
        //}

        SetHPText();
        SetHPBar();
        SetLVText();
        SetExpText();
        SetECool();
        SetRCool();
        SetTimeText();
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
        if (H_PlayerManager.instance.indexLev < H_PlayerManager.instance.maxExperiences.Length)
        {
            expBar.fillAmount = H_PlayerManager.instance.exp / H_PlayerManager.instance.maxExperiences[H_PlayerManager.instance.indexLev];
        }
        else return;
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

    public void SetTimeText()
    {
        int min = (int)(gameTime / 60);
        int sec = (int)(gameTime % 60);

        timeText.text = min.ToString() + " : " + sec.ToString();
    }
}
