using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance;

    public int defaultCapacity = 40;
    public int maxPoolSize = 100000;
    public GameObject enemyPrefab;

    public IObjectPool<GameObject> pool { get; private set; }

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
        enemyPrefab = Resources.Load<GameObject>("Enemy");
        print(enemyPrefab.name);
        Init();
    }

    private void Init()
    {
        pool = new ObjectPool<GameObject>(CreatePooled, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject,true,defaultCapacity, maxPoolSize);

        // 미리 오브젝트 생성 해놓기
        for(int i = 0; i<defaultCapacity; i++)
        {
            EnemyMove enemy = CreatePooled().GetComponent<EnemyMove>();
            enemy.pool.Release(enemy.gameObject);
        }

    }

    private GameObject CreatePooled()
    {
        print("CreatePooled");
        GameObject poolGo = Instantiate(enemyPrefab);
        poolGo.GetComponent<EnemyMove>().pool = this.pool;
        return poolGo;
    }

    private void OnTakeFromPool(GameObject poolGo)
    {
        print("OnTakeFromPool");
        poolGo.SetActive(true);
    }
    private void OnReturnedToPool(GameObject poolGo)
    {
        print("OnReturnedToPool");
        poolGo.SetActive(false);
    }
    private void OnDestroyPoolObject(GameObject poolGo)
    {
        Destroy(poolGo);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
