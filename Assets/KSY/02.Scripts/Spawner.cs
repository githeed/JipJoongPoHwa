using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;



public class Spawner : MonoBehaviour
{
    [Header("조절 가능")]
    [Tooltip("소환 주기")]
    public float coolTime;
    [Tooltip("처음 쿨 타임")]
    public float startCoolTime;
    [Tooltip("레벨 증가에 따른 쿨 감소 매개변수")]
    public float changeCoolForLev;
    float currTime;
    [Tooltip("플레이어로부터 최소 거리")]
    public float distanceMin;
    [Tooltip("플레이어로부터 최대 거리")]
    public float distanceMax;
    [Tooltip("체크 시 스폰 중지")]
    public bool StopSpawn;
    [Tooltip("여기에 소환시키고 싶은 위치 넣기")]
    public Transform[] spawnPos;
    [Tooltip("랜덤하게 소환 시키고 싶으면 체크")]
    public bool spawnRandPos;
    [Tooltip("특정한 곳에 소환 시키고 싶으면 체크")]
    public bool spawnAtSpecificPos;
    [Tooltip("소환위치에서 한 쿨타임에 소환되는 에너미 숫자")]
    public int spawnEnemyCntAtOnce;
    Transform player;
    Vector2 distance;
    float rand;
    float randDirX;
    float randDirZ;
    Vector3 dir;
    [Header("터치 금지")]
    public Transform leftTopTr;
    public Transform rightBottomTr;
    Vector3 leftTop;
    Vector3 rightBottom;
    int spawnCnt = 0;


    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        distance = new Vector2(distanceMin, distanceMax);
        if (leftTopTr == null) leftTopTr = GameObject.Find("LeftTop").transform;
        if (rightBottomTr == null) rightBottomTr = GameObject.Find("RightBottom").transform;
        leftTop = leftTopTr.position;
        rightBottom = rightBottomTr.position;
        
    }

    void Update()
    {
        if (StopSpawn || GameManager.instance.isStop) return;
        if (spawnRandPos) // 플레이어 기준 랜덤한 곳에서 소환.
        {
            currTime += Time.deltaTime;  
            if (currTime >= coolTime)
            {
                RandomSpawn();
            }
        }
        if (spawnAtSpecificPos) // 특정한 곳에서 소환
        {
            currTime += Time.deltaTime;
            if (currTime >= coolTime)
            {
                SpawnPosAtPoint();
                currTime = 0;
                coolTime = Random.Range(3f, 8f);
            }
        }
    }

    public void SpawnPosAtPoint() // 특정한 포지션에서 소환되게 해주는 함수.
    {
        if(spawnPos.Length == 0)
        {
            Debug.LogError("SpawnPos 리스트에 스폰시키고 싶은 위치의 게임오브젝트 넣어주세요");
        }
        for(int i = 0; i < spawnPos.Length; i++)
        {
            for(int j = 0; j < spawnEnemyCntAtOnce; j++)
            {
                if (spawnPos[i].gameObject.activeSelf)
                {
                    GameObject go = ObjectPoolManager.instance.pool.Get();
                    go.GetComponent<EnemyMove>().OnNav();
                    go.transform.position = spawnPos[i].transform.position;
                }

            }
        }
    }

    public void RandomSpawn()
    {
        
        spawnCnt++;
        currTime = 0;
        rand = Random.Range(distanceMin, distanceMax);
        randDirX = Random.Range(0, 1f);
        randDirZ = Random.Range(0, 1f);
        while (randDirX == 0 && randDirZ == 0)
        {
            randDirX = Random.Range(0, 1f);
            randDirZ = Random.Range(0, 1f);
        }
        switch (spawnCnt % 4)
        {
            case 0:
                randDirX *= -1;
                break;
            case 1:
                randDirX *= -1;
                randDirZ *= -1;
                break;
            case 2:
                randDirZ *= -1;
                break;
            case 3:
                break;
            default:
                break;
        }
        dir = new Vector3(randDirX, 0, randDirZ);


        GameObject go = ObjectPoolManager.instance.pool.Get();
        go.GetComponent<EnemyMove>().OnNav();
        go.transform.position = player.position + (dir.normalized * rand) + Vector3.up * 0.1f;
        if (go.transform.position.x < leftTop.x)
        {
            //go.transform.position = new Vector3(leftTop.x, go.transform.position.y, go.transform.position.z);
            go.transform.position -= Vector3.right * go.transform.position.x * 2;
        }
        if (go.transform.position.z > leftTop.z)
        {
            go.transform.position -= Vector3.forward * go.transform.position.z * 2;
        }
        if (go.transform.position.x > rightBottom.x)
        {
            go.transform.position -= Vector3.right * go.transform.position.x * 2;
        }
        if (go.transform.position.z < rightBottom.z)
        {
            go.transform.position -= Vector3.forward * go.transform.position.z * 2;
        }

        
    }
    public void SetPos(GameObject go)
    {
        rand = Random.Range(distanceMin, distanceMax);
        randDirX = Random.Range(0, 1f);
        randDirZ = Random.Range(0, 1f);
        while (randDirX == 0 && randDirZ == 0)
        {
            randDirX = Random.Range(0, 1f);
            randDirZ = Random.Range(0, 1f);
        }
        switch (spawnCnt % 4)
        {
            case 0:
                randDirX *= -1;
                break;
            case 1:
                randDirX *= -1;
                randDirZ *= -1;
                break;
            case 2:
                randDirZ *= -1;
                break;
            case 3:
                break;
            default:
                break;
        }
        dir = new Vector3(randDirX, 0, randDirZ);

        if (go.transform.position.x < leftTop.x)
        {
            //go.transform.position = new Vector3(leftTop.x, go.transform.position.y, go.transform.position.z);
            go.transform.position -= Vector3.right * go.transform.position.x * 2;
        }
        if (go.transform.position.z > leftTop.z)
        {
            go.transform.position -= Vector3.forward * go.transform.position.z * 2;
        }
        if (go.transform.position.x > rightBottom.x)
        {
            go.transform.position -= Vector3.right * go.transform.position.x * 2;
        }
        if (go.transform.position.z < rightBottom.z)
        {
            go.transform.position -= Vector3.forward * go.transform.position.z * 2;
        }
    }
}
