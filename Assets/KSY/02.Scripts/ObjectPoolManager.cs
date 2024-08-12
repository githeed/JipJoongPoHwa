using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance;

    public int defaultCapacity = 40;
    public int defaultFeatherCap = 100;
    public int maxPoolSize = 100000;
    public GameObject enemyPrefab;
    public GameObject damageUIPrefab;
    public GameObject featherPrefab;
    public GameObject ally;

    public enum ObjectPoolName
    {
        Enemy,
        DamageUI
    }

    public IObjectPool<GameObject> pool { get; private set; }
    public IObjectPool<GameObject> damageUIPool { get; private set; }
    public IObjectPool<GameObject> featherPool { get; private set; }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        
        Init();
    }

    //private void Start()
    //{
    //    ally = GameObject.Find("Ally");
    //}

    private void Init()
    {
        if (ally == null) ally = GameObject.FindWithTag("Player1");
        pool = new ObjectPool<GameObject>(CreatePooledEnemy, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject,true,defaultCapacity, maxPoolSize);
        damageUIPool = new ObjectPool<GameObject>(CreateDamagePool, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject,true,defaultCapacity, maxPoolSize);
        featherPool = new ObjectPool<GameObject>(CreateFeatherPool, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, defaultCapacity, maxPoolSize);

        // 미리 오브젝트 생성 해놓기
        for (int i = 0; i<defaultCapacity; i++)
        {
            EnemyMove enemy = CreatePooledEnemy().GetComponent<EnemyMove>();
            enemy.pool.Release(enemy.gameObject);
        }

        for (int i = 0; i < defaultFeatherCap; i++)
        {
            var feather = CreateFeatherPool();
            ally.GetComponent<Y_PlayerAttack>().pool.Release(feather.gameObject);
        }

    }
    private GameObject CreateDamagePool()
    {
        GameObject damageUI = Instantiate(damageUIPrefab);
        damageUI.GetComponent<EnemyDamageUI>().damageUIPool = this.damageUIPool;
        return damageUI;
    }

    private GameObject CreatePooledEnemy()
    {
        GameObject poolGo = Instantiate(enemyPrefab);
        poolGo.GetComponent<EnemyMove>().pool = this.pool;
        return poolGo;
    }

    private GameObject CreateFeatherPool()
    {
        GameObject feather = Instantiate(featherPrefab);
        ally.GetComponent<Y_PlayerAttack>().pool = this.featherPool;
        return feather;
    }

    private void OnTakeFromPool(GameObject poolGo)
    {
        poolGo.SetActive(true);
    }
    private void OnReturnedToPool(GameObject poolGo)
    {
        poolGo.SetActive(false);
    }
    private void OnDestroyPoolObject(GameObject poolGo)
    {
        Destroy(poolGo);
    }

}
