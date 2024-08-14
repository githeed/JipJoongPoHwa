using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;



public class Y_Feather : MonoBehaviour
{
    GameObject ally;
    public IObjectPool<GameObject> pool;
    // Start is called before the first frame update
    void Start()
    {
        ally = GameObject.Find("Ally");
        pool = ally.GetComponent<Y_PlayerAttack>().pool;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StopDestroy()
    {
        StopAllCoroutines();
    }

    public void StartDestroy(float time)
    {
        StartCoroutine(AutoDestroy(time));
    }

    IEnumerator AutoDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        pool.Release(gameObject);
    }
}
