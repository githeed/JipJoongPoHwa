﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_ParticleTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnParticleCollision(GameObject other)
    {
        print(other.gameObject.name);
    }

    private void OnParticleTrigger()
    {
        print("!!!");
    }
}
