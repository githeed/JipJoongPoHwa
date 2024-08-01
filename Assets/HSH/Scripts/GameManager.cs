using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;


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
        
    }

    void Update()
    {
        
    }
}
