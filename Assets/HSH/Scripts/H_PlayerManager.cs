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

    public float effScale = 0.3f;

    Coroutine pc;

    private void Awake()
    {
        // 만약에 instance 에 값이 없다면
        if (instance == null)
        {
            // instance 에 값을 셋팅
            instance = this;
        }
        // 그렇지 않다면
        else
        {
            // 나의 게임오브젝트를 파괴하자.
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

        weaponBtn = cardButton.GetComponent<Button>();
        img = cardButton.GetComponent<Image>();
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
        xBox += 2;
        boxDist++;
        effScale++;
        bIsPicking = false;
        print("endpick");
        weaponBtn.enabled = false;
        img.enabled = false;
    }

    public void CardPickingButton()
    {
        StopCoroutine(pc);
        xBox += 2;
        boxDist++;
        effScale++;
        Time.timeScale = 1;
        bIsPicking = false;
        weaponBtn.enabled = false;
        img.enabled = false;
    }
}
