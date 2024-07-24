using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;



public class Spawner : MonoBehaviour
{
    public static Spawner Instance;

    private Dictionary<System.Type, IObjectPool<Enemy>> pools;

    [SerializeField]
    Enemy enemy;
    [SerializeField]
    private Enemy1 enemy1Prefab;
    [SerializeField]
    private Enemy2 enemy2Prefab;
    [SerializeField]
    private Enemy3 enemy3Prefab;

    [SerializeField]
    private Transform playerTransform; // 플레이어의 위치를 추적하는 변수

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        InitializePools();
    }

    private void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    private void InitializePools()
    {
        pools = new Dictionary<System.Type, IObjectPool<Enemy>>();

        pools[typeof(Enemy1)] = new ObjectPool<Enemy>(() => CreateEnemy(enemy1Prefab), OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, 10, 20);
        pools[typeof(Enemy2)] = new ObjectPool<Enemy>(() => CreateEnemy(enemy2Prefab), OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, 10, 20);
        pools[typeof(Enemy3)] = new ObjectPool<Enemy>(() => CreateEnemy(enemy3Prefab), OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, 10, 20);
    }

    private Enemy CreateEnemy(Enemy prefab)
    {
        Enemy enemy = Instantiate(prefab);
        return enemy;
    }

    private void OnTakeFromPool(Enemy enemy)
    {
        enemy.OnObjectSpawn();
    }

    private void OnReturnedToPool(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }

    public T GetEnemy<T>() where T : Enemy
    {
        T enemy = pools[typeof(T)].Get() as T;
        PositionEnemyNearPlayer(enemy);
        return enemy;
    }

    public void ReleaseEnemy(Enemy enemy)
    {
        pools[enemy.GetType()].Release(enemy);
    }

    private void PositionEnemyNearPlayer(Enemy enemy)
    {
        // 플레이어를 기준으로 적의 랜덤 위치를 설정
        if (playerTransform != null)
        {
            float randomDistance = Random.Range(5f, 10f); // 랜덤 거리 설정
            float randomAngle = Random.Range(0f, 360f); // 랜덤 각도 설정

            Vector3 spawnDirection = new Vector3(Mathf.Cos(randomAngle * Mathf.Deg2Rad), 0, Mathf.Sin(randomAngle * Mathf.Deg2Rad)).normalized;
            Vector3 spawnPosition = playerTransform.position + spawnDirection * randomDistance;

            enemy.transform.position = spawnPosition;
        }
    }
}
