using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class H_PlayerManager : MonoBehaviour
{
    public static H_PlayerManager instance;

    public float[] maxExperiences = new float[16];
    public int indexLev = 0;
    public int expMultiplier = 5;

    public float exp = 1;

    public float maxExp = 0;

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
    }

    void Update()
    {
        
    }

    public void UpdateExp(float value)
    {
        value += exp;
        if (value >= maxExperiences[indexLev])
        {
            indexLev++;
        }
    }
}
