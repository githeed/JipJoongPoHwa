using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyDamageUI : MonoBehaviour
{
    public IObjectPool<GameObject> damageUIPool { get; set; }

    public TextMeshProUGUI amountUI;
    float startSize = 2;
    float finalSize = 1;
    float size;

    float currTime;
    public float disableTime = 1f;

    private void OnEnable()
    {
        currTime = 0;
        size = startSize;
    }


    void Update()
    {
        currTime += Time.deltaTime;
        if(currTime > disableTime)
        {
            damageUIPool.Release(gameObject);
        }
        transform.localScale = Vector3.one * size;
        transform.forward = Camera.main.transform.forward;
        if(size > finalSize)
        {
            size -= (startSize - finalSize) * Time.deltaTime;
        }
    }

    public void UpdateAmount(float dmg)
    {
        amountUI.text = ((int)dmg).ToString();
    }
}
