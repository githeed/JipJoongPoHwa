using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class H_PlayerManager : MonoBehaviour
{
    public static H_PlayerManager instance;
    public GameObject cardButton;
    Button weaponBtn;
    Image img;
    int briarNum = 1;

    public GameObject cuteButton;
    Button cuteBtn;
    Image cuteImg;
    public Image cuteSkillBox;
    int cuteNum = 0;

    public bool isCuteOn = false;

    public Image bg;
    public Image e_UI;

    public float attTime = 7;
    public float attDelay = 0.3f;
    public float curAttDelay = 0;

    public float cuteAttTime = 5;
    public float cuteCurAttDelay = 0;

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

    // 경험치 오브젝트풀
    public List<GameObject> exps = new List<GameObject>();
    public GameObject expFac;

    public Image playerHPBar;

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
        indexLev = 1;
        //print("Manager");

        weaponBtn = cardButton.GetComponent<Button>();
        img = cardButton.GetComponent<Image>();

        cuteBtn = cuteButton.GetComponent<Button>();
        cuteImg = cuteButton.GetComponent<Image>();
        //ChangeAlpha(0);

        curAttDelay = attTime;

        for (int i = 0; i < 20; i++)
        {
            GameObject ef = Instantiate(expFac);
            ef.SetActive(false);
            exps.Add(ef);
        }
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
                rCool = false;
            }
        }
        PlayerHPBar();
    }

    public void UpdateExp(float value)
    {
        if (indexLev > maxExperiences.Length) return;
        exp += value;
        if (exp >= maxExperiences[indexLev])
        {
            if(briarNum < 5)
            {
                weaponBtn.enabled = true;
                img.enabled = true;
            }
            if (cuteNum < 5)
            {
                cuteBtn.enabled = true;
                cuteImg.enabled = true;

            }

            bg.enabled = true;
            bIsPicking = true;
            indexLev++;
            exp = 0;
            pc = StartCoroutine(PickCard(10));
        }
    }

    IEnumerator PickCard(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        ImgShow();
        BriarCardPick();
    }

    public void CardPickingButton()
    {
        StopCoroutine(pc);
        ImgShow();
        BriarCardPick();
    }

    public void CuteCardPickingButton()
    {
        StopCoroutine(pc);
        cuteNum++;
        if(!isCuteOn)
        {
            cuteSkillBox.enabled = true;
            isCuteOn = true;
        }
        else
        {
            ImgShow();
            CuteCardPick();
        }
    }

    void BriarCardPick()
    {
        xBox += boxMultiplier;
        boxDist += distMultiplier;
        effScale += effMultiplier;
        attTime = curAttDelay;
        attTime -= attDelay;
        curAttDelay = attTime;
        briarNum++;
    }

    public float cuteCool = 1;
    void CuteCardPick()
    {
        cuteAttTime -= cuteCool;
    }

    void ImgShow()
    {
        Time.timeScale = 1;
        bIsPicking = false;
        weaponBtn.enabled = false;
        img.enabled = false;
        cuteBtn.enabled = false;
        cuteImg.enabled = false;
        bg.enabled = false;
    }
    //public void ChangeAlpha(float alpha)
    //{
    //    e_UI.color = new Color(1, 0, 0, alpha);
    //}

    GameObject GetExp()
    {
        GameObject exp = null;
        if (exps.Count > 0)
        {
            exp = exps[0];
            exp.SetActive(true);
            exps.RemoveAt(0);
        }
        else
        {
            exp = Instantiate(expFac);
        }

        return exp;
    }

    public void SpawnExp(Vector3 loc)
    {
        GameObject ge = GetExp();
        ge.transform.position = loc;
    }

    void PlayerHPBar()
    {
        playerHPBar.fillAmount = curHP / maxHP;
    }
}
