using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour
{
    // 적 오브젝트가 풀에서 스폰될 때 호출됩니다.
    public virtual void OnObjectSpawn()
    {
        gameObject.SetActive(true);
        // 공통 초기화 코드
    }

    private void OnDisable()
    {
        if (Spawner.Instance != null)
        {
            Spawner.Instance.ReleaseEnemy(this);
        }
    }
}

public class Enemy1 : Enemy
{
    public override void OnObjectSpawn()
    {
        base.OnObjectSpawn();
        // Enemy1 초기화 코드
    }
}

public class Enemy2 : Enemy
{
    public override void OnObjectSpawn()
    {
        base.OnObjectSpawn();
        // Enemy2 초기화 코드
    }
}

public class Enemy3 : Enemy
{
    public override void OnObjectSpawn()
    {
        base.OnObjectSpawn();
        // Enemy3 초기화 코드
    }
}
